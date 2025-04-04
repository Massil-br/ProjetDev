using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Shared;
using System.Collections.Generic;
using SFML.System;

public class Client
{
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
    private const int Port = 12345;
    private string playerId;
    private Dictionary<int, Player> otherPlayers = new Dictionary<int, Player>();

    public Client(string serverIp)
    {
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), Port);
        playerId = RequestPlayerId();
    }

    private string RequestPlayerId()
    {
        try
        {
            byte[] requestData = Encoding.UTF8.GetBytes("REQUEST_ID");
            udpClient.Send(requestData, requestData.Length, serverEndPoint);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.Receive(ref remoteEP);
            playerId = Encoding.UTF8.GetString(receivedData);
            Console.WriteLine($"Received ID from server: {playerId}");
            return playerId;
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
            throw;
        }
    }

    public void Start(Player player)
    {
        new Thread(() => ReceiveUpdates(player)).Start();

        while (true)
        {
            SendPlayerInfo(player);
            Thread.Sleep(10); // Send position every 100ms
        }
    }

    private void SendPlayerInfo(Player player)
    {
        string message = $"{playerId}:{player.GetPosition().X}:{player.GetPosition().Y}  : {player.GetIntAnimation()} : {player.IsFacingRight()}";
        byte[] sendData = Encoding.UTF8.GetBytes(message);
        udpClient.Send(sendData, sendData.Length, serverEndPoint);
    }

    private void ReceiveUpdates(Player player)
    {
        while (true)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpClient.Receive(ref remoteEP);
                string response = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine($"Position update: {response}");
                ProcessServerData(response, player);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data: {ex.Message}");
            }
        }
    }

    private void ProcessServerData(string data, Player player)
    {
        string[] playersData = data.Split('|');
        foreach (string playerData in playersData)
        {
            string[] parts = playerData.Split(':');
            if (parts.Length == 5)
            {
                string id = parts[0];
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                int intAnimation = int.Parse(parts[3]);
                bool isFacingRight = bool.Parse(parts[4]);
                

                if (id != playerId) // Do not update our own position
                {
                    Console.WriteLine($"Player {id} is at ({x}, {y})");
                    int playerIdInt = int.Parse(id);
                    if (!otherPlayers.ContainsKey(playerIdInt))
                    {
                        otherPlayers[playerIdInt] = new Player("OtherPlayer", 100, 10);
                    }
                    otherPlayers[playerIdInt].UpdatePosition(new Vector2f(x, y));
                    otherPlayers[playerIdInt].SetFacing(isFacingRight);
                    otherPlayers[playerIdInt].SetSpriteTexture(intAnimation);
                    
                }
            }
        }
    }


    

    public void Stop()
    {
        udpClient.Close();
    }

    public Dictionary<int, Player> GetOtherPlayers()
    {
        return otherPlayers;
    }
}
