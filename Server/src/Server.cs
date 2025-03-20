using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using Shared;

namespace src
{
    public class Server
    {
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();

        public Server(Ip ip)
        {
            listener = new TcpListener(IPAddress.Parse(ip.IpAddress), ip.Port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started...");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            while (true)
            {
                byte[] data = new byte[1024];
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead == 0) break;

                ServerPacket packet = ServerPacket.Deserialize(data.Take(bytesRead).ToArray());
                BroadcastPacket(packet);
            }
            clients.Remove(client);
            client.Close();
        }

        private void BroadcastPacket(ServerPacket packet)
        {
            byte[] data = packet.Serialize();
            foreach (var client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}