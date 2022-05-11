using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SecureApp
{
    public partial class TFADialog : Form
    {
        private string secret;
        public TFADialog()
        {
            InitializeComponent();
        }

        public TFADialog(string secret)
        {
            InitializeComponent();
            this.secret = secret;
            tfaLabel.Text = $"We require you to use 2FA for authentication.\nEnter this code into an authenticator app of your choice\nthen enter the generated token into the box below.\n{secret}";
            tfaLabel.Left = (this.ClientSize.Width - tfaLabel.Width) / 2;
            tfaLabel.Top = (this.ClientSize.Height - tfaLabel.Height) / 2;
        }

        public string Token { get; internal set; }

        private void tfaButton_Click(object sender, EventArgs e)
        {
            this.Token = tokenTextBox.Text;
            this.Close();
        }
    }
}
