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
            if (!client.Connect(serverAddress, port))
                return;
            if (!client.Login(username, password))
                return;
            client.WaitForEvents();
        }
    }
}
