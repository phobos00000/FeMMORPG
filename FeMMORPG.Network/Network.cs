using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace FeMMORPG.Common
{
    public class Network
    {
        public static void Send(TcpClient client, Packet packet)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(client.GetStream(), packet);
        }

        public static void Send(TcpClient client, Commands command, List<object> parameters = null)
        {
            var packet = new Packet
            {
                Command = command,
                Parameters = parameters
            };
            Send(client, packet);
        }

        public static Packet Receive(TcpClient client)
        {
            var formatter = new BinaryFormatter();
            var packet = (Packet)formatter.Deserialize(client.GetStream());
            return packet;
        }

        public static void Connect(TcpClient client, string host, int port)
        {
            client.Connect(host, port);
        }

        public static void Disconnect(TcpClient client)
        {
            client.Close();
        }
    }
}
