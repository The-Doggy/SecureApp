
namespace SecureApp
{
    partial class SignUpForm
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
            this.ShowPasswordPictureBox = new System.Windows.Forms.PictureBox();
            this.WelcomeLabel = new System.Windows.Forms.Label();
            this.LoginLink = new System.Windows.Forms.LinkLabel();
            this.LoginButton = new System.Windows.Forms.Button();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.UsernameErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.PasswordErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ShowPasswordPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsernameErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // ShowPasswordPictureBox
            // 
            this.ShowPasswordPictureBox.Location = new System.Drawing.Point(514, 244);
            this.ShowPasswordPictureBox.Name = "ShowPasswordPictureBox";
            this.ShowPasswordPictureBox.Size = new System.Drawing.Size(33, 31);
            this.ShowPasswordPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ShowPasswordPictureBox.TabIndex = 10;
            this.ShowPasswordPictureBox.TabStop = false;
            this.ShowPasswordPictureBox.Click += new System.EventHandler(this.ShowPasswordPictureBox_Click);
            // 
            // WelcomeLabel
            // 
            this.WelcomeLabel.AutoSize = true;
            this.WelcomeLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WelcomeLabel.Location = new System.Drawing.Point(256, 41);
            this.WelcomeLabel.Name = "WelcomeLabel";
            this.WelcomeLabel.Size = new System.Drawing.Size(289, 96);
            this.WelcomeLabel.TabIndex = 8;
            this.WelcomeLabel.Text = "Sign Up";
            // 
            // LoginLink
            // 
            this.LoginLink.AutoSize = true;
            this.LoginLink.Location = new System.Drawing.Point(294, 373);
            this.LoginLink.Name = "LoginLink";
            this.LoginLink.Size = new System.Drawing.Size(213, 25);
            this.LoginLink.TabIndex = 9;
            this.LoginLink.TabStop = true;
            this.LoginLink.Text = "Already have an account?";
            this.LoginLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LoginLink_LinkClicked);
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(344, 307);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(112, 34);
            this.LoginButton.TabIndex = 7;
            this.LoginButton.Text = "Sign Up";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(293, 244);
            this.PasswordTextBox.MaxLength = 64;
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.PlaceholderText = "Password";
            this.PasswordTextBox.Size = new System.Drawing.Size(215, 31);
            this.PasswordTextBox.TabIndex = 6;
            this.PasswordTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PasswordTextBox_Validating);
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(293, 179);
            this.UsernameTextBox.MaxLength = 64;
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.PlaceholderText = "Username";
            this.UsernameTextBox.Size = new System.Drawing.Size(215, 31);
            this.UsernameTextBox.TabIndex = 5;
            this.UsernameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.UsernameTextBox_Validating);
            // 
            // UsernameErrorProvider
            // 
            this.UsernameErrorProvider.ContainerControl = this;
            // 
            // PasswordErrorProvider
            // 
            this.PasswordErrorProvider.ContainerControl = this;
            // 
            // SignUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ShowPasswordPictureBox);
            this.Controls.Add(this.WelcomeLabel);
            this.Controls.Add(this.LoginLink);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.UsernameTextBox);
            this.Name = "SignUpForm";
            this.Text = "SignUpForm";
            ((System.ComponentModel.ISupportInitialize)(this.ShowPasswordPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsernameErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ShowPasswordPictureBox;
        private System.Windows.Forms.Label WelcomeLabel;
        private System.Windows.Forms.LinkLabel LoginLink;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.ErrorProvider UsernameErrorProvider;
        private System.Windows.Forms.ErrorProvider PasswordErrorProvider;
    }
}