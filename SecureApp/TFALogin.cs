using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SecureApp
{
    public partial class TFALogin : Form
    {
        public TFALogin()
        {
            InitializeComponent();
        }

        public string Token { get; internal set; }

        private void submitButton_Click(object sender, EventArgs e)
        {
            this.Token = tokenTextBox.Text;
            this.Close();
        }
    }
}
