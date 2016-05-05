using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using FeMMORPG.Common;
using FeMMORPG.Data;

namespace FeMMORPG.Server
{
    public class GameClient : IDisposable
    {
        public Server Server;
        public TcpClient Client;
        public DateTime ConnectTime;
        public DateTime LastActivity;
        public User User;

        private Persistence persistence = new Persistence();
        private bool lastPingReturned;
        private readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(10);
        private readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(120);
        private readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(30);

        public GameClient(TcpClient client)
        {
            Client = client;
        }

        public void Listen()
        {
            var connectTimer = new Timer(ConnectTimeout.TotalMilliseconds);
            connectTimer.Elapsed += ConnectTimer_Elapsed;
            connectTimer.Enabled = true;

            while (Client.Connected)
            {
                try
                {
                    var packet = Network.Receive(Client);

                    switch (packet.Command)
                    {
                        case Commands.Login: OnLogin(packet.Parameters); break;
                        case Commands.Logout: OnLogout(); break;
                        case Commands.PingAck: OnPingAck(); break;
                        default:
                            Console.WriteLine($"Unexpected command byte: {(int)packet.Command}");
                            Network.Disconnect(Client);
                            break;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Socket error: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in client loop: " + e);
                    Network.Disconnect(Client);
                }
            }

            connectTimer.Dispose();
        }

        private void ConnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (User == null)
                Network.Disconnect(Client);
        }

        private void initPingTimer()
        {
            var pingTimer = new System.Timers.Timer(PingTimeout.TotalMilliseconds);
            pingTimer.AutoReset = true;
            pingTimer.Elapsed += PingTimer_Elapsed;
            pingTimer.Enabled = true;
        }

        private void PingTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            if (!lastPingReturned)
            {
                Network.Disconnect(Client);
            }
            else if (Client.Connected)
            {
                lastPingReturned = false;
                Network.Send(Client, Commands.Ping);
            }
            else
            {
                ((System.Timers.Timer)source).Dispose();
            }
        }

        private void OnLogin(List<object> parameters)
        {
            if (parameters.Count < 3)
                Network.Disconnect(Client);

            var username = (string)parameters[0];
            var tokenId = (Guid)parameters[1];
            var clientVersion = (Version)parameters[2];
            var ip = Server.ServerAddress.ToString();
            var success = true;
            var error = ErrorCodes.None;

            // validate login token
            var token = Server.UnitOfWork.LoginTokens.Query(new[] { "user" })
                .Where(t => t.Id == tokenId)
                .Where(t => t.User.Username == username)
                .Where(t => t.ServerIP == ip)
                .FirstOrDefault();
            if (token == null || DateTime.UtcNow - token.LoginTime > TimeSpan.FromSeconds(10))
                Network.Disconnect(Client);

            if (Server.MinimumClientVersion > clientVersion)
            {
                success = false;
                error = ErrorCodes.GameVersionTooLow;
            }
            else if (Server.MaxConnectionsPerIP > 0 &&
                Server.Clients.Count(c => c.IPAddress == this.IPAddress) > Server.MaxConnectionsPerIP)
            {
                success = false;
                error = ErrorCodes.MaxConnectionsExceeded;
            }
            else if (Server.MaxConnectionsPerAccount > 0 &&
                Server.Clients.Count(c => c.User != null && c.User.Username == username) > Server.MaxConnectionsPerAccount)
            {
                success = false;
                error = ErrorCodes.MaxConnectionsExceeded;
            }

            Network.Send(Client, Commands.LoginAck, new List<object> { success, error });

            if (!success)
                Network.Disconnect(Client);

            Console.WriteLine("User " + User.Username + " has logged in");
            lastPingReturned = true;
            User = token.User;
            User.LastLogin = DateTime.UtcNow;
            Server.UnitOfWork.LoginTokens.Remove(token);
            initPingTimer();
        }

        private void OnLogout()
        {
            Network.Disconnect(Client);
        }

        private void OnPingAck()
        {
            lastPingReturned = true;
        }

        private void Logout(ErrorCodes reason)
        {
            if (Client.Connected)
            {
                Network.Send(Client, Commands.Logout, new List<object> { reason });
            }
            Network.Disconnect(Client);
        }

        public string IPAddress
        {
            get
            {
                if (Client.Connected)
                    return ((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString();
                return "";
            }
        }

        public void Dispose()
        {
            Network.Disconnect(Client);
        }
    }
}
