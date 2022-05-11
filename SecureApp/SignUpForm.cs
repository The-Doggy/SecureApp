using SecureRemotePassword;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net.Security;
using System.Net.Sockets;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using TwoFactorAuthNet;
using System.Windows.Forms;
using System.IO;

namespace SecureApp
{
    public partial class SignUpForm : Form
    {
        private static ResourceManager rm = new ResourceManager("SecureApp.Properties.Resources", typeof(LoginForm).Assembly);
        private static readonly SrpClient client = new SrpClient();

        public SignUpForm()
        {
            InitializeComponent();
            ShowPasswordPictureBox.Image = (Image)rm.GetObject("invisible.png");
        }

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

        private void LoginButton_Click(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void LoginLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadLoginForm();
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
            if (ValidateUsername() && ValidatePassword())
            {
                TryRegister(UsernameTextBox.Text.ToLower(), PasswordTextBox.Text);
            }
            else
            {
                MessageBox.Show(this, "Please fix errors and try again.", "Invalid Input", MessageBoxButtons.OK);
            }
        }

        private void TryRegister(string user, string pass)
        {
            var salt = client.GenerateSalt();
            var privateKey = client.DerivePrivateKey(salt, user, pass);
            var verifier = client.DeriveVerifier(privateKey);
            string server = File.ReadAllText("server.txt");

            TcpClient tcp = new TcpClient(server, 8080);
            SslStream stream = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(CheckCert));
            stream.AuthenticateAsClient(server);

            // Send info to server using REGISTER: to signal what type of message we're sending, <EOL> to signal the next part of the message and <EOF> to signal end of message
            // Using this type of designation means that the data sent always needs to be in the correct order otherwise the server will read incorrect data
            byte[] message = Encoding.UTF8.GetBytes($"REGISTER:{user}<EOL>{salt}<EOL>{verifier}<EOF>");
            stream.Write(message);
            stream.Flush();

            string serverMessage = ReadMessage(stream);

            // Get the rest of the string without "REGISTER:" at the start and "<EOF>" at the end
            serverMessage = serverMessage[(serverMessage.IndexOf(':') + 1)..];
            serverMessage = serverMessage.Remove(serverMessage.IndexOf("<EOF>"));
            Debug.WriteLine(serverMessage);

            // If all we get back is "0" the registration attempt failed
            if (serverMessage.Equals("0"))
            {
                MessageBox.Show(this, $"Failed to register user {user}", "Registration Failed", MessageBoxButtons.OK);
                tcp.Close();
                return;
            }

            // Registration has completed successfully, we can now setup 2FA
            TwoFactorAuth tfa = new TwoFactorAuth("SecureApp");
            string secret = tfa.CreateSecret(160);
            using (TFADialog dialog = new TFADialog(secret))
            {
                // Do not allow client to proceed until they authenticate with 2FA
                bool tfaAuthenticated = false;
                do
                {
                    dialog.ShowDialog();
                    string token = dialog.Token;
                    if (tfa.VerifyCode(secret, token))
                    {
                        // Token is valid, send secret to server to store
                        message = Encoding.UTF8.GetBytes($"REGISTER:{secret}<EOF>");
                        stream.Write(message);
                        stream.Flush();

                        serverMessage = ReadMessage(stream);

                        // Get the rest of the string without "REGISTER:" at the start and "<EOF>" at the end
                        serverMessage = serverMessage[(serverMessage.IndexOf(':') + 1)..];
                        serverMessage = serverMessage.Remove(serverMessage.IndexOf("<EOF>"));
                        Debug.WriteLine(serverMessage);

                        // If all we get back is "1" the registration attempt succeeded
                        if (serverMessage.Equals("1"))
                        {
                            tfaAuthenticated = true;
                        }
                    }
                    if (!tfaAuthenticated)
                    {
                        MessageBox.Show(this, $"Failed to verify 2FA for user {user}", "Registration Failed", MessageBoxButtons.OK);
                    }
                }
                while (!tfaAuthenticated);
            }

            // User has been registered successfully
            MessageBox.Show(this, $"Successfully registered user {user}", "Registration Successful", MessageBoxButtons.OK);

            // Close the connection
            tcp.Close();

            // Go back to the login form
            LoadLoginForm();
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

        private void Setup2FAComponents()
        {
            UsernameTextBox.Hide();
            PasswordTextBox.Hide();
            LoginButton.Hide();
            LoginLink.Hide();
        }

        private void LoadLoginForm()
        {
            // This is all we need to do here since the login form should still be loaded and we have a delegate to switch back to it once this form closes
            this.Close();
        }
    }
}
