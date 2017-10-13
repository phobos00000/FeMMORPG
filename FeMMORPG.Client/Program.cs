using System;
using System.Configuration;

namespace FeMMORPG.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];
            if (serverAddress == null)
                throw new ArgumentNullException(nameof(serverAddress));

            var serverPort = ConfigurationManager.AppSettings["serverPort"];
            if (serverPort == null)
                throw new ArgumentNullException(nameof(serverPort));
            var port = int.Parse(serverPort);

            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            var client = new Client();
            client.CharactersReceived += OnCharactersReceived;
            if (!client.Connect(serverAddress, port, username, password))
                return;
            client.WaitForEvents();
        }

        static void OnCharactersReceived(object sender, CharacterEventArgs e)
        {
            Console.WriteLine("Received character list:");
            Console.WriteLine("------------------------");
            e.Characters.ForEach(c => Console.WriteLine($"[{c.Id}] {c.Name}"));
        }
    }
}
