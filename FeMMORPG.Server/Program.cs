using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;

namespace FeMMORPG.Server
{
    class Program
    {
        private static IPAddress address;
        private static int port;

        static void Main(string[] args)
        {
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["serverAddress"], out address))
                address = IPAddress.Parse("0.0.0.0");

            if (!int.TryParse(ConfigurationManager.AppSettings["serverPort"], out port))
                port = 4895;

            ThreadPool.QueueUserWorkItem(CreateServer);

            var running = true;
            while (running)
            {
                Console.Write(": ");
                var command = Console.ReadLine();
                var parts = command.Split(' ');

                switch (parts[0])
                {
                    case "clients":
                        ShowClients();
                        break;
                    case "kick":
                        var client = Server.GetInstance().Clients
                            .Where(c => c.User.Id == parts[1] || c.IPAddress.ToString() == parts[1])
                            .SingleOrDefault();
                        if (client != null)
                            client.Dispose();
                        else
                            Console.WriteLine($"User {parts[1]} could not be found");
                        break;
                    case "q":
                    case "quit":
                    case "exit":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }

            Console.WriteLine("Shutting down...");
            Server.GetInstance().Stop();
        }

        private static void ShowClients()
        {
            Console.WriteLine("Active Clients:");
            Console.WriteLine("{0,14} | {1,18} | {2}", "IP", "Logged In", "User");
            Console.WriteLine("------------------------------------------");
            Server.GetInstance().Clients.ForEach(client => Console.WriteLine(string.Format(
                "{0,14} | {1,18} | {2}",
                ((IPEndPoint)client.Client.Client.RemoteEndPoint).Address,
                (DateTime.UtcNow - client.ConnectTime).ToString(@"hh\:mm\:ss"),
                client.User?.Id)));
        }

        private static void CreateServer(object state)
        {
            var server = Server.GetInstance();
            server.ServerAddress = address;
            server.ServerPort = port;
            server.Listen();
        }
    }
}
