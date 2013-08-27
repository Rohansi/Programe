using System;
using System.Windows.Forms;

namespace Programe.Forms
{
    public partial class LoginForm : Form
    {
        public string Username
        {
            get { return UsernameBox.Text; }
        }

        public string Password
        {
            get { return PasswordBox.Text; }
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void PasswordBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
