using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FeMMORPG.Common
{
    public class Network
    {
        public const int BufferSize = 128;

        public static void Send(TcpClient client, Packet packet)
        {
            var formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(client.GetStream(), packet);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize object, reason: " + e.Message);
            }
        }

        public static Packet Receive(TcpClient client)
        {
            var formatter = new BinaryFormatter();
            try
            {
                var packet = (Packet)formatter.Deserialize(client.GetStream());
                return packet;
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize object, reason: " + e.Message);
                throw;
            }
        }

        public static void Connect(TcpClient client, string host, int port)
        {
            try
            {
                client.Connect(host, port);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to connect to {host}:{port}, reason: {e.Message}");
                throw;
            }
        }

        public static void Disconnect(TcpClient client)
        {
            try
            {
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while attempting to disconnect: " + e.Message);
            }
        }
    }
}
