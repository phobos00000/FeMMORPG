using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public event PacketEventHandler PacketReceived;
        public event LoginEventHandler LoggedIn;
        public event LogoutEventHandler LoggedOut;

        private Persistence persistence = new Persistence();
        private bool lastPingReturned;
        private MapperConfiguration mapConfig;
        private Timer connectTimer;
        private Timer pingTimer;
        private readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(10);
        private readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(120);
        private readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(30);

        public GameClient(TcpClient client)
        {
            Client = client;
            mapConfig = MapConfig.Config();

            PacketReceived += ProcessPacket;
            LoggedIn += OnLogin;
            LoggedOut += OnLogout;

            connectTimer = new Timer(ConnectTimeout.TotalMilliseconds);
            connectTimer.Elapsed += OnConnectTimer;

            pingTimer = new Timer(PingTimeout.TotalMilliseconds);
            pingTimer.AutoReset = true;
            pingTimer.Elapsed += OnPingTimer;
        }

        public void Listen()
        {
            connectTimer.Enabled = true;

            while (Client.Connected)
            {
                try
                {
                    var packet = Network.Receive(Client);
                    OnPacketReceived(new PacketEventArgs(packet.Command, packet.Parameters));
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

            OnLoggedOut(new LogoutEventArgs(User, DateTime.UtcNow));
        }

        internal void FakePacket(Packet packet)
        {
            OnPacketReceived(new PacketEventArgs(packet.Command, packet.Parameters));
        }

        private void ProcessPacket(object sender, PacketEventArgs e)
        {
            // If user is not logged in, sending anything other than login command
            // will terminate the connection.
            if (((GameClient)sender).User == null && e.Command != Commands.Login)
            {
                Network.Disconnect(Client);
                return;
            }

            switch (e.Command)
            {
                case Commands.Login: HandleLogin(e.Parameters); break;
                case Commands.Logout: HandleLogout(); break;
                case Commands.Ping: HandlePing(); break;
                case Commands.Played: HandlePlayed(); break;
                default:
                    Console.WriteLine($"Unexpected command byte: {(int)e.Command}");
                    Network.Disconnect(Client);
                    break;
            }
        }

        private void OnLogin(object sender, LoginEventArgs e)
        {
            Console.WriteLine($"User {e.User.Username} logged in");

            User = e.User;

            // Save login date
            User.LastLogin = e.LoginTime;
            Server.UnitOfWork.Users.Update(User);
            Server.UnitOfWork.SaveChanges();

            // Turn on heartbeat
            lastPingReturned = true;
            pingTimer.Enabled = true;
        }

        private void OnLogout(object sender, LogoutEventArgs e)
        {
            var connectTime = e.LogoutTime - User.LastLogin.Value;
            Console.WriteLine($"User {e.User.Username} logged out, connect time: {connectTime.ToString(@"hh\:mm\:ss")}");

            // Save logout date and total time played this session
            e.User.LastLogout = e.LogoutTime;
            e.User.SecondsPlayed += (int)connectTime.TotalSeconds;
            Server.UnitOfWork.Users.Update(e.User);
            Server.UnitOfWork.SaveChanges();

            // Dispose timers only relevant when connected
            pingTimer.Dispose();
            connectTimer.Dispose();
        }

        private void OnConnectTimer(object sender, ElapsedEventArgs e)
        {
            if (User == null)
                Network.Disconnect(Client);
        }

        private void OnPingTimer(object sender, ElapsedEventArgs e)
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
                ((Timer)sender).Enabled = false;
            }
        }

        private void HandleLogin(List<object> parameters)
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
            var token = Server.UnitOfWork.LoginTokens.Query(new[] { "user", "user.characters" })
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
                Server.Clients.Count(c => c.User != null && c.User.Username == username) + 1 > Server.MaxConnectionsPerAccount)
            {
                success = false;
                error = ErrorCodes.MaxConnectionsExceeded;
            }

            if (success)
            {
                var characters = token.User.Characters.AsQueryable().ProjectTo<Common.Models.Character>(mapConfig).ToList();
                Network.Send(Client, Commands.Login, new List<object> { true, characters });
                OnLoggedIn(new LoginEventArgs(token.User, DateTime.UtcNow));
            }
            else
            {
                Network.Send(Client, Commands.Login, new List<object> { false, error });
                Network.Disconnect(Client);
            }

            Server.UnitOfWork.LoginTokens.Remove(token);
            Server.UnitOfWork.SaveChanges();
        }

        private void HandleLogout()
        {
            Network.Disconnect(Client);
        }

        private void HandlePing()
        {
            lastPingReturned = true;
        }

        private void HandlePlayed()
        {
            var played = User.SecondsPlayed + (int)((DateTime.UtcNow - User.LastLogin.Value).TotalSeconds);
            Network.Send(Client, Commands.Played, new List<object> { played });
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

        protected virtual void OnPacketReceived(PacketEventArgs e)
        {
            if (PacketReceived != null)
                PacketReceived(this, e);
        }

        protected virtual void OnLoggedIn(LoginEventArgs e)
        {
            if (LoggedIn != null)
                LoggedIn(this, e);
        }

        protected virtual void OnLoggedOut(LogoutEventArgs e)
        {
            if (LoggedOut != null)
                LoggedOut(this, e);
        }

        public void Dispose()
        {
            Network.Disconnect(Client);
        }
    }
}
