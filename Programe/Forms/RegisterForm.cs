using System;
using System.Linq;
using System.Windows.Forms;

namespace Programe.Forms
{
    public partial class RegisterForm : Form
    {
        public string Username
        {
            get { return UsernameBox.Text; }
        }

        public string Password
        {
            get { return PasswordBox.Text; }
        }

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, EventArgs e)
        {
            var user = UsernameBox.Text;
            if (user.Length < 3 || user.Length > 16 || !user.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("Usernames must be 3 to 16 characters long and may only contain letters, digits and whitespace.", "Register");
                return;
            }

            if (PasswordBox.Text.Length < 6)
            {
                MessageBox.Show("Passwords must be at least 6 characters long.", "Register");
                return;
            }

            if (PasswordBox.Text != PasswordConfirm.Text)
            {
                MessageBox.Show("The passwords do not match.", "Register");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
