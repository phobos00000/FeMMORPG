using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FeMMORPG.Common;
using FeMMORPG.Data;

namespace FeMMORPG.Server
{
    public class Server
    {
        private TcpListener socket;
        private CancellationTokenSource cancelTokenSource;

        public IUserUnitOfWork UnitOfWork;
        public IPAddress ServerAddress;
        public int ServerPort;
        public List<GameClient> Clients = new List<GameClient>();
        public Version MinimumClientVersion;
        public int MaxConnections;
        public int MaxConnectionsPerIP;
        public int MaxConnectionsPerAccount;

        public Server(IPAddress address, int port, CancellationTokenSource cts)
        {
            ServerAddress = address;
            ServerPort = port;
            cancelTokenSource = cts;
            UnitOfWork = new UserUnitOfWork(new UserDbContext());
            MinimumClientVersion = new Version(ConfigurationManager.AppSettings["minClientVersion"]);
            if (!int.TryParse(ConfigurationManager.AppSettings["maxConnections"], out MaxConnections))
                MaxConnections = 100;
            if (!int.TryParse(ConfigurationManager.AppSettings["maxConnectionsPerIp"], out MaxConnectionsPerIP))
                MaxConnectionsPerIP = 0;
            if (!int.TryParse(ConfigurationManager.AppSettings["maxConnectionsPerAccount"], out MaxConnectionsPerAccount))
                MaxConnectionsPerAccount = 1;
        }

        public void Listen()
        {
            Console.WriteLine("Starting...");
            socket = new TcpListener(ServerAddress, ServerPort);
            socket.Start();

            var server = new Data.Server
            {
                IP = ServerAddress.ToString(),
                CurrentUsers = 0,
                MaxUsers = MaxConnections,
                Enabled = true,
            };

            UnitOfWork.Servers.Add(server);
            UnitOfWork.SaveChanges();

            Console.WriteLine("Server is listening for new connections");

            while (!cancelTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(200);
                if (socket.Pending())
                {
                    var tcpClient = socket.AcceptTcpClient();
                    Task.Factory.StartNew(AcceptClient, tcpClient, TaskCreationOptions.LongRunning);
                }
            }

            server = UnitOfWork.Servers.Find(ServerAddress.ToString());
            UnitOfWork.Servers.Remove(server);
            UnitOfWork.SaveChanges();

            Console.WriteLine("Server has received a request to shut down");
            socket.Stop();
            Clients.ForEach(c => Network.Disconnect(c.Client));
        }

        private void AcceptClient(object obj)
        {
            Console.WriteLine("A client connection has been established");

            UnitOfWork.Servers.Find(ServerAddress.ToString());
            var client = new GameClient((TcpClient)obj);
            client.Server = this;
            client.ConnectTime = DateTime.UtcNow;
            this.Clients.Add(client);
            client.Listen();
            this.Clients.Remove(client);
            client.Dispose();

            Console.WriteLine("A client connection has been terminated");
        }
    }
}
