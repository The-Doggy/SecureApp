using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SecureApp
{
    public partial class MainForm : Form
    {
        private string username;
        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(string username)
        {
            this.username = username;
        }
    }
}
