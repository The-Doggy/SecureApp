
namespace SecureApp
{
    partial class TFADialog
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
            this.WelcomeLabel = new System.Windows.Forms.Label();
            this.tokenTextBox = new System.Windows.Forms.TextBox();
            this.tfaLabel = new System.Windows.Forms.Label();
            this.tfaButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WelcomeLabel
            // 
            this.WelcomeLabel.AutoSize = true;
            this.WelcomeLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WelcomeLabel.Location = new System.Drawing.Point(221, 38);
            this.WelcomeLabel.Name = "WelcomeLabel";
            this.WelcomeLabel.Size = new System.Drawing.Size(359, 96);
            this.WelcomeLabel.TabIndex = 9;
            this.WelcomeLabel.Text = "2FA Setup";
            // 
            // tokenTextBox
            // 
            this.tokenTextBox.Location = new System.Drawing.Point(201, 307);
            this.tokenTextBox.Name = "tokenTextBox";
            this.tokenTextBox.PlaceholderText = "Token";
            this.tokenTextBox.Size = new System.Drawing.Size(379, 31);
            this.tokenTextBox.TabIndex = 10;
            // 
            // tfaLabel
            // 
            this.tfaLabel.AutoSize = true;
            this.tfaLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tfaLabel.Location = new System.Drawing.Point(145, 164);
            this.tfaLabel.Name = "tfaLabel";
            this.tfaLabel.Size = new System.Drawing.Size(231, 32);
            this.tfaLabel.TabIndex = 11;
            this.tfaLabel.Text = "PLEASE CHANGE ME";
            this.tfaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tfaButton
            // 
            this.tfaButton.Location = new System.Drawing.Point(344, 376);
            this.tfaButton.Name = "tfaButton";
            this.tfaButton.Size = new System.Drawing.Size(112, 34);
            this.tfaButton.TabIndex = 12;
            this.tfaButton.Text = "Submit";
            this.tfaButton.UseVisualStyleBackColor = true;
            this.tfaButton.Click += new System.EventHandler(this.tfaButton_Click);
            // 
            // TFADialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tfaButton);
            this.Controls.Add(this.tfaLabel);
            this.Controls.Add(this.tokenTextBox);
            this.Controls.Add(this.WelcomeLabel);
            this.Name = "TFADialog";
            this.Text = "TFADialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label WelcomeLabel;
        private System.Windows.Forms.TextBox tokenTextBox;
        private System.Windows.Forms.Label tfaLabel;
        private System.Windows.Forms.Button tfaButton;
    }
}