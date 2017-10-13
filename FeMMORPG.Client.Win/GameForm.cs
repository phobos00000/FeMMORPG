using System.Collections.Generic;
using System.Windows.Forms;
using FeMMORPG.Common.Models;

namespace FeMMORPG.Client.Win
{
    public partial class GameForm : Form
    {
        public static GameForm Instance { get; private set; }
        private Client client;

        public GameForm(Client client)
        {
            InitializeComponent();

            Instance = this;
            this.client = client;

            Initialize();
        }

        private void Initialize()
        {
            // size window, initialize dx or whatever
        }

        public void CharacterSelect(List<Character> characters)
        {
            characters.ForEach(c => lstCharacters.Items.Add(c.Name));
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
