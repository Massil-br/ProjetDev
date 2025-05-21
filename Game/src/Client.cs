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
        PlayerServerInfo Pinfo = new(player);
        PlayerMessage message = new(Pinfo, playerId);
        Console.WriteLine($"Envoi des données du joueur: {message.Message}");
        byte[] sendData = Encoding.UTF8.GetBytes(message.Message);
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
        if (string.IsNullOrEmpty(data))
        {
            Console.WriteLine("Données reçues vides");
            return;
        }

        Console.WriteLine($"Données brutes reçues: {data}");

        string[] parts = data.Split('|');
        if (parts.Length == 0) return;

        // Traitement de l'état de la salle
        string roomState = parts[0];
        Console.WriteLine($"État de la salle: {roomState}");

        // Si on est en attente, on ne traite pas les positions des joueurs
        if (roomState == "STATE=WAITING")
        {
            Console.WriteLine("En attente d'autres joueurs...");
            return;
        }

        // Traitement des données des joueurs
        for (int i = 1; i < parts.Length; i++)
        {
            string p = parts[i];
            if (string.IsNullOrEmpty(p)) continue;
            
            Console.WriteLine($"Traitement des données du joueur: {p}");
            
            string[] playerParts = p.Split(':');
            if (playerParts.Length == 8 && int.TryParse(playerParts[0], out int id))
            {
                try
                {
                    float x = float.Parse(playerParts[1]);
                    float y = float.Parse(playerParts[2]);
                    Animation anim = Enum.Parse<Animation>(playerParts[3]);
                    bool facing = bool.Parse(playerParts[4]);
                    float vertical = float.Parse(playerParts[5]);
                    float moveX = float.Parse(playerParts[6]);
                    float moveY = float.Parse(playerParts[7]);

                    Console.WriteLine($"Données parsées - ID: {id}, Position: ({x}, {y}), Animation: {anim}");

                    if (id != playerId)
                    {
                        if (!otherPlayers.ContainsKey(id))
                        {
                            Console.WriteLine($"Nouveau joueur détecté: {id}");
                            otherPlayers[id] = new Player("OtherPlayer", 100, 10);
                        }

                        Player op = otherPlayers[id];
                        op.UpdatePosition(new Vector2f(x, y));
                        op.SetFacing(facing);
                        op.SetAnimationState(anim);
                        op.SetVerticalSpeed(vertical);
                        op.SetPlayerMovement(new Vector2f(moveX, moveY));
                        Console.WriteLine($"Joueur {id} mis à jour - Position: ({x}, {y})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du traitement des données du joueur {id}: {ex.Message}");
                    Console.WriteLine($"Données problématiques: {p}");
                }
            }
            else
            {
                Console.WriteLine($"Format de données invalide: {p}");
                Console.WriteLine($"Nombre de parties: {playerParts.Length}");
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
