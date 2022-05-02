namespace SecureApp
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.SignUpLink = new System.Windows.Forms.LinkLabel();
            this.WelcomeLabel = new System.Windows.Forms.Label();
            this.UsernameErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.PasswordErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ShowPasswordPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.UsernameErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShowPasswordPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(293, 180);
            this.UsernameTextBox.MaxLength = 64;
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.PlaceholderText = "Username";
            this.UsernameTextBox.Size = new System.Drawing.Size(215, 31);
            this.UsernameTextBox.TabIndex = 0;
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(293, 243);
            this.PasswordTextBox.MaxLength = 64;
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.PlaceholderText = "Password";
            this.PasswordTextBox.Size = new System.Drawing.Size(215, 31);
            this.PasswordTextBox.TabIndex = 1;
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(344, 306);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(112, 34);
            this.LoginButton.TabIndex = 2;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // SignUpLink
            // 
            this.SignUpLink.AutoSize = true;
            this.SignUpLink.Location = new System.Drawing.Point(363, 372);
            this.SignUpLink.Name = "SignUpLink";
            this.SignUpLink.Size = new System.Drawing.Size(75, 25);
            this.SignUpLink.TabIndex = 3;
            this.SignUpLink.TabStop = true;
            this.SignUpLink.Text = "Sign Up";
            this.SignUpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SignUpLink_LinkClicked);
            // 
            // WelcomeLabel
            // 
            this.WelcomeLabel.AutoSize = true;
            this.WelcomeLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WelcomeLabel.Location = new System.Drawing.Point(233, 52);
            this.WelcomeLabel.Name = "WelcomeLabel";
            this.WelcomeLabel.Size = new System.Drawing.Size(334, 96);
            this.WelcomeLabel.TabIndex = 3;
            this.WelcomeLabel.Text = "Welcome";
            // 
            // UsernameErrorProvider
            // 
            this.UsernameErrorProvider.ContainerControl = this;
            // 
            // PasswordErrorProvider
            // 
            this.PasswordErrorProvider.ContainerControl = this;
            // 
            // ShowPasswordPictureBox
            // 
            this.ShowPasswordPictureBox.Location = new System.Drawing.Point(514, 243);
            this.ShowPasswordPictureBox.Name = "ShowPasswordPictureBox";
            this.ShowPasswordPictureBox.Size = new System.Drawing.Size(33, 31);
            this.ShowPasswordPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ShowPasswordPictureBox.TabIndex = 4;
            this.ShowPasswordPictureBox.TabStop = false;
            this.ShowPasswordPictureBox.Click += new System.EventHandler(this.ShowPasswordPictureBox_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ShowPasswordPictureBox);
            this.Controls.Add(this.WelcomeLabel);
            this.Controls.Add(this.SignUpLink);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.UsernameTextBox);
            this.Name = "LoginForm";
            this.Text = "Secure App";
            ((System.ComponentModel.ISupportInitialize)(this.UsernameErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShowPasswordPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.LinkLabel SignUpLink;
        private System.Windows.Forms.Label WelcomeLabel;
        private System.Windows.Forms.ErrorProvider UsernameErrorProvider;
        private System.Windows.Forms.PictureBox ShowPasswordPictureBox;
        private System.Windows.Forms.ErrorProvider PasswordErrorProvider;
    }
}