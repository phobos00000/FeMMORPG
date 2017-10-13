using System;
using System.Windows.Forms;

namespace FeMMORPG.Client.Win
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            picLoader.Visible = true;
            Program.Client.Connect(Program.LoginServer.Address, Program.LoginServer.Port,
                txtUsername.Text, txtPassword.Text);
            picLoader.Visible = false;
        }
    }
}
