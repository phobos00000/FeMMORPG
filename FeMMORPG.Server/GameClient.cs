using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using FeMMORPG.Common;
using FeMMORPG.Synchronization;

namespace FeMMORPG.Server
{
    public class GameClient : IDisposable
    {
        public TcpClient Client { get; set; }
        public DateTime ConnectTime { get; set; }
        public User User { get; set; }
        private bool connected;
        private Persistence persistence;
        private bool respondedToPing;

        public void Listen()
        {

            persistence = new Persistence();
            connected = true;
            respondedToPing = true;
            var idleTimer = new Timer(2 * 60 * 1000);
            idleTimer.AutoReset = true;
            idleTimer.Elapsed += OnIdleTimer;
            idleTimer.Enabled = true;

            while (connected)
            {
                var packet = new Packet();
                try
                {
                    packet = Network.Receive(Client);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Socket error: " + e.Message);
                    if (!Client.Connected)
                        break;
                }

                Console.WriteLine($"Received {packet.Command.ToString()} packet with {packet.Parameters.Count} params");
                Console.WriteLine(packet.Parameters.DefaultIfEmpty(string.Empty).Aggregate((i, j) => i.ToString() + ", " + j.ToString()));

                try
                {
                    switch (packet.Command)
                    {
                        case Commands.Connect:
                            OnConnect(packet.Parameters);
                            break;

                        case Commands.Login:
                            OnLogin(packet.Parameters);
                            break;

                        case Commands.Logout:
                            OnLogout();
                            break;

                        case Commands.PingAck:
                            OnPingAck();
                            break;

                        default:
                            Console.WriteLine($"Unexpected command byte: {(int)packet.Command}");
                            connected = false;
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in client loop: " + e);
                }
            }

            this.Dispose();
        }

        private void OnConnect(List<object> parameters)
        {
            var canConnect = true;
            var connectError = ErrorCodes.None;
            if (Server.GetInstance().MinimumVersion.CompareTo(parameters[0]) > 0)
            {
                canConnect = false;
                connectError = ErrorCodes.GameVersionTooLow;
            }
            else if (Server.GetInstance().Clients.Count(c => c.IPAddress == this.IPAddress) > Server.MaxConnectionsPerIP)
            {
                canConnect = false;
                connectError = ErrorCodes.MaxConnectionsExceeded;
            }

            Network.Send(Client, new Packet
            {
                Command = Commands.ConnectAck,
                Parameters = { canConnect, connectError }
            });
        }

        private void OnLogin(List<object> parameters)
        {
            var username = (string)parameters[0];
            var password = (string)parameters[1];

            var success = true;
            var loginError = ErrorCodes.None;
            var user = persistence.GetUser(username);
            if (user == null || user.Password != password)
            {
                success = false;
                loginError = ErrorCodes.InvalidLogin;

            }
            else if (Server.GetInstance().Clients.Any(c => c.User != null && c.User.Id == username))
            {
                success = false;
                loginError = ErrorCodes.UserAlreadyLoggedIn;
            }

            Network.Send(Client, new Packet
            {
                Command = Commands.LoginAck,
                Parameters = { success, loginError }
            });

            if (success)
            {
                this.User = user;
                this.User.LastLogin = DateTime.UtcNow;
                persistence.SaveUser(this.User);
            }
            else
                connected = false;
        }

        private void OnLogout()
        {
            Dispose();
        }

        private void OnIdleTimer(object source, ElapsedEventArgs e)
        {
            if (respondedToPing == false)
            {
                Logout(ErrorCodes.UserIdleTimeout);
            }

            if (connected)
            {
                respondedToPing = false;
                Network.Send(Client, new Packet
                {
                    Command = Commands.Ping,
                });
            }
            else
            {
                ((Timer)source).Dispose();
            }
        }

        private void OnPingAck()
        {
            respondedToPing = true;
        }

        private void Logout(ErrorCodes reason)
        {
            if (Client.Connected)
            {
                Network.Send(Client, new Packet
                {
                    Command = Commands.Logout,
                    Parameters = { reason }
                });
            }
            Dispose();
        }

        public IPAddress IPAddress
        {
            get
            {
                return ((IPEndPoint)Client.Client.RemoteEndPoint).Address;
            }
        }

        public void Dispose()
        {
            connected = false;
            Network.Disconnect(Client);
        }
    }
}
