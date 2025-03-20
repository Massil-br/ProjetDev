using System.Net.Sockets;
using Shared;

namespace src{
    public class Client{
        private TcpClient client;
        private NetworkStream stream;


        public Client(Ip ip){
            client = new(ip.IpAddress, ip.Port);
            stream = client.GetStream();
        }

        public void SendPacket(ServerPacket packet)
        {
            byte[] data = packet.Serialize();
            stream.Write(data, 0, data.Length);
        }

        public ServerPacket ReceivePacket()
        {
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            return ServerPacket.Deserialize(data.Take(bytesRead).ToArray());
        }

        public void Close()
        {
            stream.Close();
            client.Close();
        }



    }
}