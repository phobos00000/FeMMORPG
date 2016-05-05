using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FeMMORPG.Server
{
    class Program
    {
        private static IPAddress address;
        private static int port;
        private static Server server;
        private static CancellationTokenSource tokenSource;

        static void Main(string[] args)
        {
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["serverAddress"], out address))
                address = IPAddress.Parse("0.0.0.0");

            if (!int.TryParse(ConfigurationManager.AppSettings["serverPort"], out port))
                port = 4895;

            tokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                tokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            Task.Factory.StartNew(ReadCommands, TaskCreationOptions.LongRunning);

            Console.WriteLine("Starting server...");
            server = new Server(address, port, tokenSource);
            server.Listen();
            Console.WriteLine("Server shut down");
        }

        private static void ReadCommands()
        {
            while (!tokenSource.IsCancellationRequested)
            {
                var command = Console.ReadLine();
                ParseCommand(command);
            }
        }

        private static void ParseCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            var parts = command.Split(' ');

            switch (parts[0])
            {
                case "clients":
                case "list":
                    ShowClients();
                    break;
                case "kick":
                    KickClient(parts.Skip(1).ToArray());
                    break;
                case "q":
                case "quit":
                case "exit":
                    tokenSource.Cancel();
                    break;
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }

        private static void KickClient(string[] clients)
        {
            foreach (var part in clients)
            {
                var client = server.Clients
                    .Where(c => c.User.Username == part || c.IPAddress.ToString() == part)
                    .SingleOrDefault();
                if (client != null)
                    client.Dispose();
                else
                    Console.WriteLine($"User {part} could not be found");
            }
        }

        private static void ShowClients()
        {
            Console.WriteLine("Active Clients:");
            Console.WriteLine("{0,14} | {1,18} | {2}", "IP", "Logged In", "User");
            Console.WriteLine("------------------------------------------");
            server.Clients.ForEach(client => Console.WriteLine(string.Format(
                "{0,14} | {1,18} | {2}",
                client.IPAddress,
                (DateTime.UtcNow - client.ConnectTime).ToString(@"hh\:mm\:ss"),
                client.User?.Username)));
        }
    }
}
