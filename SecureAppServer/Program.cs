using MySqlConnector;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using SecureRemotePassword;
using System.Text;

namespace SecureAppServer
{
    public sealed class Program
    {
        private static readonly MySqlConnection conn = new MySqlConnection("server=162.248.93.113;user=test_user;database=SecureAppDB;");
        private static readonly SrpServer server = new SrpServer();

        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            RunServer(args[0]);
        }

        static X509Certificate2 serverCertificate;
        // The certificate parameter specifies the name of the file
        // containing the machine certificate.
        public static void RunServer(string certificatePath)
        {
            using (X509Store store = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);

                X509Certificate2Collection coll = new X509Certificate2Collection();
                coll.Import(certificatePath);

                foreach (X509Certificate2 cert in coll)
                {
                    if (cert.HasPrivateKey)
                    {
                        // Maybe apply more complex logic if you really expect multiple private-key certs.
                        if (serverCertificate == null)
                        {
                            serverCertificate = cert;
                        }
                        else
                        {
                            cert.Dispose();
                        }
                    }
                    else
                    {
                        // This handles duplicates (as long as no custom properties have been applied using MMC)
                        store.Add(cert);
                        cert.Dispose();
                    }
                }
            }

            // Set password for MySQL connection
            Func<MySqlProvidePasswordContext, string> getPass = context => File.ReadAllText("pass.txt");
            conn.ProvidePasswordCallback = getPass;

            // Create a TCP/IP (IPv4) socket and listen for incoming connections.
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for a client to connect...");
                // Application blocks while waiting for an incoming connection.
                // Type CNTL-C to terminate the server.
                TcpClient client = listener.AcceptTcpClient();
                ProcessClient(client);
            }
        }
        static void ProcessClient(TcpClient client)
        {
            // A client has connected. Create the
            // SslStream using the client's network stream.
            SslStream sslStream = new SslStream(
                client.GetStream(), false);
            // Authenticate the server but don't require the client to authenticate.
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, false);
                // Set timeouts for the read and write to 5 seconds.
                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;
                // Read a message from the client.
                Console.WriteLine("Waiting for client message...");
                string messageData = ReadMessage(sslStream);
                //Console.WriteLine("Received: {0}", messageData);

                // Check what type of message has been received:
                if(messageData.StartsWith("REGISTER:"))
                {
                    // Get the rest of the string without "REGISTER:" at the start and "<EOF>" at the end
                    messageData = messageData[(messageData.IndexOf(':') + 1)..];
                    messageData = messageData.Remove(messageData.IndexOf("<EOF>"));

                    // Split the data from each <EOL> section
                    string[] registerInfo = messageData.Split("<EOL>", StringSplitOptions.RemoveEmptyEntries);
                    foreach(string data in registerInfo)
                    {
                        Console.WriteLine(data);
                    }

                    // Write user info to database
                    conn.Open();
                    using MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT IGNORE INTO users (username, salt, verifier) VALUES (@u, @s, @v)";
                    cmd.Parameters.AddWithValue("u", registerInfo[0]);
                    cmd.Parameters.AddWithValue("s", registerInfo[1]);
                    cmd.Parameters.AddWithValue("v", registerInfo[2]);
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();

                    // Send result to client
                    byte[] message = Encoding.UTF8.GetBytes($"REGISTER:{result}<EOF>");
                    sslStream.Write(message);
                }
                else if(messageData.StartsWith("LOGIN:"))
                {
                    // Get the rest of the string without "LOGIN:" at the start and "<EOF>" at the end
                    messageData = messageData[(messageData.IndexOf(':') + 1)..];
                    messageData = messageData.Remove(messageData.IndexOf("<EOF>"));

                    // Split the data from each <EOL> section
                    string[] userInfo = messageData.Split("<EOL>", StringSplitOptions.RemoveEmptyEntries);
                    foreach (string data in userInfo)
                    {
                        Console.WriteLine(data);
                    }
                    string user = userInfo[0];
                    string clientPublicEphemeral = userInfo[1];

                    // Get user info from database
                    conn.Open();
                    using MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT salt, verifier FROM users WHERE username = @u";
                    cmd.Parameters.AddWithValue("u", userInfo[0]);
                    using MySqlDataReader reader = cmd.ExecuteReader();
                    string salt = null;
                    string verifier = null;
                    if(reader.Read())
                    {
                        salt = reader.GetString(0);
                        verifier = reader.GetString(1);
                    }
                    conn.Close();

                    byte[] message;
                    SrpEphemeral serverEphemeral = null;
                    if (salt == null || verifier == null)
                    {
                        message = Encoding.UTF8.GetBytes("LOGIN:0<EOF>");
                        sslStream.Write(message);
                        sslStream.Close();
                        client.Close();
                        return;
                    }
                    else
                    {
                        serverEphemeral = server.GenerateEphemeral(verifier);
                        message = Encoding.UTF8.GetBytes($"LOGIN:{salt}<EOL>{serverEphemeral.Public}<EOF>");
                        sslStream.Write(message);
                        sslStream.Flush();
                    }

                    Console.WriteLine("Waiting for client message...");
                    messageData = ReadMessage(sslStream);
                    Console.WriteLine($"Received: {messageData}");

                    // Get the rest of the string without "LOGIN:" at the start and "<EOF>" at the end
                    messageData = messageData[(messageData.IndexOf(':') + 1)..];
                    messageData = messageData.Remove(messageData.IndexOf("<EOF>"));

                    try
                    {
                        SrpSession serverSession = server.DeriveSession(serverEphemeral.Secret, clientPublicEphemeral, salt, user, verifier, messageData);
                        message = Encoding.UTF8.GetBytes($"LOGIN:{serverSession.Proof}<EOF>");
                        sslStream.Write(message);
                        sslStream.Flush();
                    }
                    catch(SecurityException)
                    {
                        // Client session proof is invalid, send login failure to client
                        message = Encoding.UTF8.GetBytes("LOGIN:0<EOF>");
                        sslStream.Write(message);
                        sslStream.Flush();
                    }
                }
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                sslStream.Close();
                client.Close();
                return;
            }
            finally
            {
                // The client stream will be closed with the sslStream
                // because we specified this behavior when creating
                // the sslStream.
                sslStream.Close();
                client.Close();
            }
        }
        static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the client.
            // The client signals the end of the message using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                // Read the client's test message.
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }
    }
}
