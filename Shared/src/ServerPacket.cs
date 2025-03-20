

using System.Text.Json;

namespace Shared{
    public class ServerPacket{
        public User User { get; private set; }
        public Ip Ip { get; private set; }
        public Player Player { get; private set; }
        public Player Player2 {get; private set;}


        public ServerPacket(User user, Ip ip, Player player, Player player2){
            User = user ?? throw new ArgumentNullException(nameof(user));
            Ip = ip ?? throw new ArgumentNullException(nameof(ip));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            
            Player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
        }

        public byte[] Serialize()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            return System.Text.Encoding.UTF8.GetBytes(jsonString);
        }

        public static ServerPacket Deserialize(byte[] data)
        {
            string jsonString = System.Text.Encoding.UTF8.GetString(data);
            var packet = JsonSerializer.Deserialize<ServerPacket>(jsonString);

            if (packet == null || packet.User == null || packet.Ip == null || packet.Player == null || packet.Player2 == null)
            {
                throw new ArgumentException("Deserialization failed due to null values.");
            }

            return packet;
        }

        


    }
}