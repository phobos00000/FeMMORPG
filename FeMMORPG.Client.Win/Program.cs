using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeMMORPG.Client.Win
{
    static class Program
    {
        public static Client Client { get; private set; }
        public static LoginServer LoginServer { get; private set; }

        private static LoginForm loginForm;
        private static GameForm gameForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];
            if (serverAddress == null)
                throw new ArgumentNullException(nameof(serverAddress));

            var serverPort = ConfigurationManager.AppSettings["serverPort"];
            if (serverPort == null)
                throw new ArgumentNullException(nameof(serverPort));
            var port = int.Parse(serverPort);

            LoginServer = new LoginServer { Address = serverAddress, Port = port };
            Client = new Client();
            Client.Connected += Client_Connected;

            loginForm = new LoginForm();
            Application.Run(loginForm);

            Client.Server.Close();
        }

        private static void Client_Connected(object sender, ConnectEventArgs e)
        {
            if (e.Success)
            {
                Task.Factory.StartNew(() => Client.WaitForEvents(), TaskCreationOptions.LongRunning);
                loginForm.Hide();
                gameForm = new GameForm(Client);
                gameForm.FormClosed += (obj, args) => loginForm.Close();
                gameForm.Show();
                gameForm.CharacterSelect(e.Characters);
            }
            else
            {
                loginForm.lblError.Text = e.Error.ToString();
                loginForm.lblError.Visible = true;
            }
        }
    }

    public class LoginServer
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }
}
