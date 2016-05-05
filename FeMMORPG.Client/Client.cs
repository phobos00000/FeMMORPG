using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using FeMMORPG.Common;
using Newtonsoft.Json;

namespace FeMMORPG.Client
{
    public class Client
    {
        public const string Version = "0.1.0";
        public TcpClient Server = new TcpClient();

        public Client()
        {
        }

        public bool Connect(string serverAddress, int serverPort, string username, string password)
        {
            try
            {
                var webClient = new WebClient();
                var uri = string.Format("http://{0}:{1}/login", serverAddress, serverPort);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                var encodedPassword = Convert.ToBase64String(new SHA256Managed().ComputeHash(passwordBytes));

                var data = JsonConvert.SerializeObject(new
                {
                    username = username,
                    password = password
                });
                var response = webClient.UploadString(uri, data);
                dynamic result = JsonConvert.DeserializeObject<dynamic>(response);
                if ((bool)result.success != true)
                {
                    Console.WriteLine("Handshake failure: " + ((ErrorCodes)result.errorType).ToString());
                    return false;
                }

                var token = Guid.Parse((string)result.token);
                var ip = (string)result.serverIP;

                Console.WriteLine("Login successful, connecting to game server " + ip);
                Network.Connect(Server, ip, 4895);
                if (Server.Connected)
                {
                    Network.Send(Server, new Packet
                    {
                        Command = Commands.Login,
                        Parameters =
                        {
                            username,
                            token,
                            new Version(Version)
                        }
                    });

                    var packet = Network.Receive(Server);
                    if (packet.Command == Commands.LoginAck && (bool)packet.Parameters[0] == true)
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

        public void WaitForEvents()
        {
            while (Server.Connected)
            {
                var packet = new Packet();
                try
                {
                    packet = Network.Receive(Server);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Socket error: " + e.Message);
                    break;
                }

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
