using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using MySqlConnector;
using System.Windows.Forms;
using SecureRemotePassword;
using System.Security.Cryptography;
using Isopoh.Cryptography.Argon2;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Security;
using TwoFactorAuthNet;

namespace SecureApp
{
    public partial class LoginForm : Form
    {
        private static ResourceManager rm = new ResourceManager("SecureApp.Properties.Resources", typeof(LoginForm).Assembly);
        private static ModifyRegistry registry = new ModifyRegistry();
        private static readonly SrpClient client = new SrpClient();
        private int loginAttempts;
        private DateTime lockoutTime;

        public LoginForm()
        {
            InitializeComponent();
            ShowPasswordPictureBox.Image = (Image)rm.GetObject("invisible.png");

            registry.BaseRegistryKey = Registry.CurrentUser;
            try
            {
                lockoutTime = DateTime.Parse(registry.Read("lockoutTime"));
            }
            catch (ArgumentNullException)
            {
                // Don't need to do anything with exception so just log it
                Debug.WriteLine("lockoutTime is null.");
            }
        }

/*      NOTE: nope
        private void UsernameTextBox_Validating(object sender, CancelEventArgs e)
        {
            // Show an error if the username is invalid
            UsernameErrorProvider.SetError(UsernameTextBox, ValidateUsername() ? "" : "Please use only letters (a-z), numbers and underscores.");
        }

        private void PasswordTextBox_Validating(object sender, CancelEventArgs e)
        {
            // Show an error if the password is invalid
            PasswordErrorProvider.SetError(PasswordTextBox, ValidatePassword() ? "" : "Password must be between 8-64 characters");
        }
*/

        private void LoginButton_Click(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void SignUpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadSignUpForm();
        }

        private void ShowPasswordPictureBox_Click(object sender, EventArgs e)
        {
            // Switch the image to whichever one is needed
            string newImage = ShowPasswordPictureBox.Image == (Image)rm.GetObject("eye.png") ? "invisible.png" : "eye.png";
            ShowPasswordPictureBox.Image = (Image)rm.GetObject(newImage);

            // Set the PasswordChar to show/unshow password text
            PasswordTextBox.PasswordChar = newImage == "eye.png" ? '0' : '*';
        }

        private bool ValidateUsername()
        {
            var filter = new ProfanityFilter.ProfanityFilter();
            return Regex.IsMatch(UsernameTextBox.Text, "^[A-Za-z0-9_]+$") && !filter.ContainsProfanity(UsernameTextBox.Text);
        }

        private bool ValidatePassword()
        {
            return PasswordTextBox.Text.Length >= 8;
        }

        private void ValidateForm()
        {
            if(ValidateUsername() && ValidatePassword())
            {
                TryLogin(UsernameTextBox.Text.ToLower(), PasswordTextBox.Text);
            }
            else
            {
                MessageBox.Show(this, "Invalid username/password.", "Invalid Details", MessageBoxButtons.OK);
            }
        }

        private void TryLogin(string user, string pass)
        {
            if (!CheckLoginAttempts()) return;

            string server = File.ReadAllText("server.txt");
            TcpClient tcp = new TcpClient(server, 8080);
            SslStream stream = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(CheckCert));
            stream.AuthenticateAsClient(server);

            // Send info to server using LOGIN: to signal what type of message we're sending, <EOL> to signal the next part of the message and <EOF> to signal end of message
            // Using this type of designation means that the data sent always needs to be in the correct order otherwise the server will read incorrect data
            SrpEphemeral clientEphemeral = client.GenerateEphemeral();
            byte[] message = Encoding.UTF8.GetBytes($"LOGIN:{user}<EOL>{clientEphemeral.Public}<EOF>");
            stream.Write(message);
            stream.Flush();

            string serverMessage = ReadMessage(stream);
            Debug.WriteLine(serverMessage);

            // Get the rest of the string without "LOGIN:" at the start and "<EOF>" at the end
            serverMessage = serverMessage[(serverMessage.IndexOf(':') + 1)..];
            serverMessage = serverMessage.Remove(serverMessage.IndexOf("<EOF>"));
            Debug.WriteLine(serverMessage);

            // If all we get back is "0" the login attempt failed
            if(serverMessage.Equals("0"))
            {
                MessageBox.Show(this, "Invalid username/password.", "Invalid Details", MessageBoxButtons.OK);
                loginAttempts++;
                tcp.Close();
                return;
            }

            // Split the data from each <EOL> section
            string[] serverData = serverMessage.Split("<EOL>", StringSplitOptions.RemoveEmptyEntries);
            foreach (string data in serverData)
            {
                Debug.WriteLine(data);
            }

            string salt = serverData[0];
            string serverPublicEphemeral = serverData[1];
            string privateKey = client.DerivePrivateKey(salt, user, pass);
            SrpSession clientSession = client.DeriveSession(clientEphemeral.Secret, serverPublicEphemeral, salt, user, privateKey);
            Debug.WriteLine(clientSession.Proof);

            message = Encoding.UTF8.GetBytes($"LOGIN:{clientSession.Proof}<EOF>");
            stream.Write(message);
            stream.Flush();

            serverMessage = ReadMessage(stream);
            Debug.WriteLine(serverMessage);

            // Get the rest of the string without "LOGIN:" at the start and "<EOF>" at the end
            serverMessage = serverMessage[(serverMessage.IndexOf(':') + 1)..];
            serverMessage = serverMessage.Remove(serverMessage.IndexOf("<EOF>"));
            Debug.WriteLine(serverMessage);

            // If all we get back is "0" the login attempt failed
            if (serverMessage.Equals("0"))
            {
                MessageBox.Show(this, "Invalid username/password.", "Invalid Details", MessageBoxButtons.OK);
                loginAttempts++;
                tcp.Close();
                return;
            }

            try
            {
                client.VerifySession(clientEphemeral.Public, clientSession, serverMessage);
            }
            catch(SecurityException)
            {
                // Server session proof is invalid, deny login
                MessageBox.Show(this, "Invalid username/password.", "Invalid Details", MessageBoxButtons.OK);
                loginAttempts++;
                tcp.Close();
                return;
            }

            // If we get here then everything else has gone right and we can now check 2FA
            message = Encoding.UTF8.GetBytes($"LOGIN:{user}<EOF>");
            stream.Write(message);
            stream.Flush();

            serverMessage = ReadMessage(stream);
            Debug.WriteLine(serverMessage);

            // Get the rest of the string without "LOGIN:" at the start and "<EOF>" at the end
            serverMessage = serverMessage[(serverMessage.IndexOf(':') + 1)..];
            serverMessage = serverMessage.Remove(serverMessage.IndexOf("<EOF>"));
            Debug.WriteLine(serverMessage);

            // If all we get back is "0" the login attempt failed
            if (serverMessage.Equals("0"))
            {
                MessageBox.Show(this, "Login failed.", "Invalid Details", MessageBoxButtons.OK);
                loginAttempts++;
                tcp.Close();
                return;
            }

            TwoFactorAuth tfa = new TwoFactorAuth("SecureApp");
            using (TFALogin dialog = new TFALogin())
            {
                // Do not allow client to proceed until they authenticate with 2FA
                bool tfaAuthenticated = false;
                do
                {
                    dialog.ShowDialog();
                    string token = dialog.Token;
                    if(tfa.VerifyCode(serverMessage, token))
                    {
                        tfaAuthenticated = true;
                    }
                }
                while (!tfaAuthenticated);
            }

            // If we get to here the client has authenticated successfully with username/password and 2FA so we can let them in
            LoadMainForm(user);

            // Close the connection
            tcp.Close();
        }

        private bool CheckLoginAttempts()
        {
            // Check if a lockout is still in effect
            if (lockoutTime > DateTime.UtcNow)
            {
                MessageBox.Show(this, "There have been too many login attempts from this device recently, " +
                    "please try again shortly.", "Login Denied", MessageBoxButtons.OK);
                return false;
            }
            else
            {
                if (loginAttempts > 5)
                {
                    // Set a lockout time of 5 minutes
                    lockoutTime = DateTime.UtcNow.AddMinutes(5d);

                    // Write the lockout time to the registry in case of application restart
                    registry.Write("lockoutTime", lockoutTime.ToString());

                    // Set login attempts back to 0
                    loginAttempts = 0;

                    // Repeat this function so that the message box is displayed to user
                    CheckLoginAttempts();
                }
            }
            return true;
        }

        public static bool CheckCert(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            // Don't care about cert authentication
            return true;
        }

        private string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        private void LoadSignUpForm()
        {
            SignUpForm form = new SignUpForm
            {
                Location = this.Location,
                StartPosition = FormStartPosition.Manual
            };
            form.FormClosing += delegate { this.Show(); };
            form.Show();
            this.Hide();
        }

        private void LoadMainForm(string username)
        {
            MainForm form = new MainForm(username)
            {
                Location = this.Location,
                StartPosition = FormStartPosition.Manual
            };
            form.FormClosing += delegate { this.Show(); };
            form.Show();
            this.Hide();
        }
    }


/* *************************************** 
 *			 ModifyRegistry.cs
 * ---------------------------------------
 *         a very simple class 
 *    to read, write, delete and count
 *       registry values with C#
 * ---------------------------------------
 *      if you improve this code 
 *   please email me your improvement!
 * ---------------------------------------
 *         by Francesco Natali
 *        - fn.varie@libero.it -
 * ***************************************/

    /// <summary>
    /// An useful class to read/write registry keys
    /// </summary>
    public class ModifyRegistry
	{
        /// <summary>
        /// A property to show or hide error messages 
        /// (default = false)
        /// </summary>
        public bool ShowError { get; set; } = false;

        /// <summary>
        /// A property to set the SubKey value
        /// (default = "SOFTWARE\\" + Application.ProductName.ToUpper())
        /// </summary>
        public string SubKey { get; set; } = "SOFTWARE\\" + Application.ProductName.ToUpper();

        /// <summary>
        /// A property to set the BaseRegistryKey value.
        /// (default = Registry.LocalMachine)
        /// </summary>
        public RegistryKey BaseRegistryKey { get; set; } = Registry.LocalMachine;

        /* **************************************************************************
		 * **************************************************************************/

        /// <summary>
        /// To read a registry key.
        /// input: KeyName (string)
        /// output: value (string) 
        /// </summary>
        public string Read(string KeyName)
		{
			// Opening the registry key
			RegistryKey rk = BaseRegistryKey;
			// Open a subKey as read-only
			RegistryKey sk1 = rk.OpenSubKey(SubKey);
			// If the RegistrySubKey doesn't exist -> (null)
			if (sk1 == null)
			{
				return null;
			}
			else
			{
				try
				{
					// If the RegistryKey exists I get its value
					// or null is returned.
					return (string)sk1.GetValue(KeyName.ToUpper());
				}
				catch (Exception e)
				{
					// AAAAAAAAAAARGH, an error!
					ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
					return null;
				}
			}
		}

		/// <summary>
		/// To write into a registry key.
		/// input: KeyName (string) , Value (object)
		/// output: true or false 
		/// </summary>
		public bool Write(string KeyName, object Value)
		{
			try
			{
				// Setting
				RegistryKey rk = BaseRegistryKey;
				// I have to use CreateSubKey 
				// (create or open it if already exits), 
				// 'cause OpenSubKey open a subKey as read-only
				RegistryKey sk1 = rk.CreateSubKey(SubKey);
				// Save the value
				sk1.SetValue(KeyName.ToUpper(), Value);

				return true;
			}
			catch (Exception e)
			{
				// AAAAAAAAAAARGH, an error!
				ShowErrorMessage(e, "Writing registry " + KeyName.ToUpper());
				return false;
			}
		}

		/* **************************************************************************
		 * **************************************************************************/

		private void ShowErrorMessage(Exception e, string Title)
		{
			if (ShowError == true)
				MessageBox.Show(e.Message,
								Title
								, MessageBoxButtons.OK
								, MessageBoxIcon.Error);
		}
	}
}
