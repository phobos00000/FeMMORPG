using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FeMMORPG.Server
{
    public class Server
    {
        private static Server instance;
        public const int MaxConnectionsPerIP = 1;
        private bool listening = false;
        private TcpListener socket;
        public IPAddress ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public List<GameClient> Clients { get; } = new List<GameClient>();
        public Version MinimumVersion { get; set; } = new Version("0.1.0");

        public Server()
        {
            Console.CancelKeyPress += delegate
            {
                this.Stop();
            };
        }

        public static Server GetInstance()
        {
            if (Server.instance == null)
                Server.instance = new Server();
            return Server.instance;
        }

        public void Listen()
        {
            socket = new TcpListener(ServerAddress, ServerPort);
            socket.Start();
            listening = true;
            Console.WriteLine("Server is listening for new connections");
            while (listening)
            {
                var tcpClient = socket.AcceptTcpClient();
                tcpClient.ReceiveBufferSize = 256;
                ThreadPool.QueueUserWorkItem(AcceptClient, tcpClient);
            }

            this.Stop();
            Console.WriteLine("Server has stopped accepting new connections");
        }

        public void Stop()
        {
            Console.WriteLine("Server has received a request to shut down");
            listening = false;
            socket.Stop();
        }

        private void AcceptClient(object obj)
        {
            Console.WriteLine("A client connection has been established");
            var client = new GameClient
            {
                Client = (TcpClient)obj,
                ConnectTime = DateTime.UtcNow,
            };
            this.Clients.Add(client);
            client.Listen();
            this.Clients.Remove(client);
            client.Dispose();
            Console.WriteLine("A client connection has been terminated");
        }
    }
}
