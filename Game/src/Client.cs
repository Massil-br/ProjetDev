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
    private int playerId;
    private Dictionary<int, Player> otherPlayers = new();
    private CancellationTokenSource cancellationTokenSource = new();

    public Client(string serverIp, int port)
    {
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
    }

    public bool StartMatchmaking(Player player, int timeoutSeconds)
    {
        try
        {
            byte[] requestData = Encoding.UTF8.GetBytes("MATCHMAKING_REQUEST");
            udpClient.Send(requestData, requestData.Length, serverEndPoint);

            udpClient.Client.ReceiveTimeout = timeoutSeconds * 1000;

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.Receive(ref remoteEP);
            string response = Encoding.UTF8.GetString(receivedData);

            if (response.StartsWith("PLAYER_ID:"))
            {
                playerId = int.Parse(response[10..]);
                Console.WriteLine($"Received Player ID: {playerId}");

                receivedData = udpClient.Receive(ref remoteEP);
                string roomResponse = Encoding.UTF8.GetString(receivedData);

                if (roomResponse.StartsWith("ROOM_ID:"))
                {
                    int roomId = int.Parse(roomResponse[8..]);
                    Console.WriteLine($"Assigned to Room ID: {roomId}");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Matchmaking failed: {ex.Message}");
        }
        return false;
    }

    public void Start(Player player)
    {
        new Thread(() => ReceiveUpdates(player, cancellationTokenSource.Token)).Start();

        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            SendPlayerInfo(player);
            Thread.Sleep(10);
        }
    }

    private void SendPlayerInfo(Player player)
    {
        string message = $"{playerId}:{player.GetPosition().X}:{player.GetPosition().Y}:" +
                         $"{player.GetAnimationState()}:{player.IsFacingRight()}:" +
                         $"{player.GetVerticalSpeed()}:{player.GetPlayerMovement().X}:{player.GetPlayerMovement().Y}";
        byte[] sendData = Encoding.UTF8.GetBytes(message);
        udpClient.Send(sendData, sendData.Length, serverEndPoint);
    }

    private void ReceiveUpdates(Player player, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpClient.Receive(ref remoteEP);
                string response = Encoding.UTF8.GetString(receivedData);
                ProcessServerData(response, player);
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive error: {ex.Message}");
            }
        }
    }

    private void ProcessServerData(string data, Player player)
    {
        string[] playersData = data.Split('|');
        foreach (string p in playersData)
        {
            string[] parts = p.Split(':');
            if (parts.Length == 8 && int.TryParse(parts[0], out int id))
            {
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                Animation anim = Enum.Parse<Animation>(parts[3]);
                bool facing = bool.Parse(parts[4]);
                float vertical = float.Parse(parts[5]);
                float moveX = float.Parse(parts[6]);
                float moveY = float.Parse(parts[7]);

                if (id != playerId)
                {
                    if (!otherPlayers.ContainsKey(id))
                        otherPlayers[id] = new Player("OtherPlayer", 100, 10);

                    Player op = otherPlayers[id];
                    op.UpdatePosition(new Vector2f(x, y));
                    op.SetFacing(facing);
                    op.SetAnimationState(anim);
                    op.SetVerticalSpeed(vertical);
                    op.SetPlayerMovement(new Vector2f(moveX, moveY));
                }
            }
        }
    }

    public void Stop()
    {
        cancellationTokenSource.Cancel();
        udpClient.Close();
    }

    public Dictionary<int, Player> GetOtherPlayers() => otherPlayers;
}
