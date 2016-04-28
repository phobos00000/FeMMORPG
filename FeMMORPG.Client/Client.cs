using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using FeMMORPG.Common;

namespace FeMMORPG.Client
{
    public class Client
    {
        public const string Version = "0.1.0";
        public TcpClient Server { get; set; }

        public Client()
        {
        }

        public bool Connect(string serverAddress, int serverPort)
        {
            try
            {
                Server = new TcpClient();
                Network.Connect(Server, serverAddress, serverPort);
                if (Server.Connected)
                {
                    Network.Send(Server, new Packet
                    {
                        Command = Commands.Connect,
                        Parameters = { new Version(Version) }
                    });

                    var packet = Network.Receive(Server);
                    if (packet.Command == Commands.ConnectAck && (bool)packet.Parameters[0] == true)
                    {
                        Console.WriteLine("Server handshake successful!");
                        return true;
                    }

                    Console.WriteLine("Server handshake failed: " + ((ErrorCodes)packet.Parameters[1]).ToString());
                    return false;
                }

                Console.WriteLine("Could not connect to server!");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                var bytes = new SHA256Managed().ComputeHash(passwordBytes);

                Network.Send(Server, new Packet
                {
                    Command = Commands.Login,
                    Parameters =
                    {
                        username,
                        Convert.ToBase64String(bytes),
                    }
                });

                var packet = Network.Receive(Server);
                if (packet.Command == Commands.LoginAck && (bool)packet.Parameters[0] == true)
                {
                    Console.WriteLine("Login successful!");
                    return true;
                }

                Console.WriteLine("Login failed: " + ((ErrorCodes)packet.Parameters[1]).ToString());
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void WaitForEvents()
        {
            while (Server.Connected)
            {
                var packet = new Packet();
                try
                {
                    packet = Network.Receive(Server);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Socket error: " + e.Message);
                    if (!Server.Connected)
                        break;
                }

                Console.WriteLine($"Received {packet.Command.ToString()} packet with {packet.Parameters.Count} params");

                try
                {
                    switch (packet.Command)
                    {
                        case Commands.Logout:
                            OnLogout();
                            break;

                        case Commands.Ping:
                            OnPing(packet.Parameters);
                            break;

                        default:
                            Console.WriteLine($"Unexpected command byte: {(int)packet.Command}");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while receiving or parsing command from server: " + e.Message);
                }

                if (!Server.Connected)
                    break;
            }

            Network.Disconnect(Server);
        }

        private void OnPing(List<object> parameters)
        {
            Network.Send(Server, new Packet
            {
                Command = Commands.PingAck,
                Parameters = parameters,
            });
        }

        private void OnLogout()
        {
            Console.WriteLine("We have been logged out");
            Network.Disconnect(Server);
        }
    }
}
