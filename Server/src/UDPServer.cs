using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using SFML.System;
using Shared;
namespace src;

public class UDPServer
{
    private UdpClient udpServer;
    private IPEndPoint clientEndPoint;
    private const int Port = 12345;
    private int nextPlayerId = 1;
    private Dictionary<int, Vector2f> players = new Dictionary<int, Vector2f>();
    private Dictionary<int, IPEndPoint> playerEndPoints = new Dictionary<int, IPEndPoint>();
    private Dictionary<int, Animation> playerAnimationState = new Dictionary<int, Animation>();
    private Dictionary<int, bool> playerFacing = new Dictionary<int, bool>();
    private Dictionary<int, float> playerVerticalSpeed = new();
    private Dictionary<int, Vector2f>playerMovement= new();
    private Dictionary<int, DateTime> lastSeen = new();
    private const int TimeoutSeconds = 5; 
   
  
    

    public UDPServer()
    {
        udpServer = new UdpClient(Port);
        clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("UDP server started on port " + Port);
    }

    public void Start()
    {
        while (true)
        {
            try
            {
                byte[] receivedData = udpServer.Receive(ref clientEndPoint);
                string message = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine("Received message: " + message);

                HandleMessage(message);
                HandleDisconnections();
                BroadcastPlayerStates();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }



    private void HandleMessage(string message)
    {
        string[] parts = message.Split(':');
        if (parts[0] == "REQUEST_ID")
        {
            AssignPlayerId();
        }
        else if (parts.Length == 8 && int.TryParse(parts[0], out int playerId))
        {
            UpdatePlayerState(playerId, parts);
        }
    }

    private void AssignPlayerId()
    {
        int assignedId = nextPlayerId++;
        byte[] idData = Encoding.UTF8.GetBytes(assignedId.ToString());
        udpServer.Send(idData, idData.Length, clientEndPoint);
        playerEndPoints[assignedId] = clientEndPoint;
        Console.WriteLine($"New player connected with ID {assignedId}");
    }

    private void UpdatePlayerState(int playerId, string[] parts)
    {
        lastSeen[playerId] = DateTime.UtcNow;
        Console.WriteLine($"last seen {playerId} : {lastSeen[playerId]}");

        float x = float.Parse(parts[1]);
        float y = float.Parse(parts[2]);
        Animation anim = Enum.Parse<Animation>(parts[3]);
        bool isFacingRight = bool.Parse(parts[4]);
        float verticalSpeed = float.Parse(parts[5]);
        float movementX = float.Parse(parts[6]);
        float movementY = float.Parse(parts[7]);

        players[playerId] = new Vector2f(x, y);
        playerAnimationState[playerId] = anim;
        playerFacing[playerId] = isFacingRight;
        playerVerticalSpeed[playerId] = verticalSpeed;
        playerMovement[playerId] = new Vector2f(movementX, movementY);
    }

    private void HandleDisconnections()
    {
        List<int> disconnectedPlayers = new();

        foreach (var kvp in lastSeen)
        {
            if ((DateTime.UtcNow - kvp.Value).TotalSeconds > TimeoutSeconds)
            {
                disconnectedPlayers.Add(kvp.Key);
            }
        }

        foreach (int id in disconnectedPlayers)
        {
           DisconnectPlayer(id);
        }
    }


    private void DisconnectPlayer(int id){
        players.Remove(id);
        playerAnimationState.Remove(id);
        playerFacing.Remove(id);
        playerVerticalSpeed.Remove(id);
        playerMovement.Remove(id);
        playerEndPoints.Remove(id);
        lastSeen.Remove(id);

        Console.WriteLine($"Player {id} disconnected due to inactivity.");

    }

    private void BroadcastPlayerStates()
    {
        StringBuilder responseBuilder = new StringBuilder();
        foreach (var kvp in players)
        {
            responseBuilder.Append($"{kvp.Key}:{kvp.Value.X}:{kvp.Value.Y}:{ 
                playerAnimationState[kvp.Key]}:{playerFacing[kvp.Key]}:{ 
                playerVerticalSpeed[kvp.Key]}:{playerMovement[kvp.Key].X}:{playerMovement[kvp.Key].Y}|");
        }

        string responseMessage = responseBuilder.ToString().TrimEnd('|');
        byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);

        foreach (var endPoint in playerEndPoints.Values)
        {
            udpServer.Send(responseData, responseData.Length, endPoint);
        }
    }

    public void Stop()
    {
        udpServer.Close();
    }
}
