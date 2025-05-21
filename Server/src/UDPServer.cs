using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SFML.System;
using Shared;

public class GameRoom
{
    public int RoomId { get; }
    public List<int> Players { get; } = new();
    public const int MaxPlayers = 2;
    public bool IsFull => Players.Count >= MaxPlayers;
    public bool IsReady => Players.Count == MaxPlayers;

    public GameRoom(int id) => RoomId = id;

    public void AddPlayer(int playerId)
    {
        if (!IsFull && !Players.Contains(playerId))
            Players.Add(playerId);
    }

    public void RemovePlayer(int playerId) => Players.Remove(playerId);
}

public class UDPServer
{
    private UdpClient udpServer = new(12345);
    private Dictionary<int, IPEndPoint> playerEndpoints = new();
    private Dictionary<int, Vector2f> playerPositions = new();
    private Dictionary<int, Animation> playerAnimations = new();
    private Dictionary<int, bool> playerFacings = new();
    private Dictionary<int, float> playerVerticalSpeeds = new();
    private Dictionary<int, Vector2f> playerMovements = new();
    private Dictionary<int, GameRoom> rooms = new();
    private Dictionary<int, int> playerToRoom = new();
    private Dictionary<int, DateTime> lastSeen = new();
    private int nextPlayerId = 1;
    private const int TimeoutSeconds = 5;

    public void Start()
    {
        Console.WriteLine("Server started on port 12345");
        while (true)
        {
            try
            {
                IPEndPoint sender = new(IPAddress.Any, 0);
                byte[] data = udpServer.Receive(ref sender);
                string msg = Encoding.UTF8.GetString(data);

                Console.WriteLine($"Message reçu: {msg}");

                if (msg == "MATCHMAKING_REQUEST")
                {
                    int id = nextPlayerId++;
                    playerEndpoints[id] = sender;
                    GameRoom room = rooms.Values.FirstOrDefault(r => !r.IsFull) ?? CreateRoom();
                    room.AddPlayer(id);
                    playerToRoom[id] = room.RoomId;

                    udpServer.Send(Encoding.UTF8.GetBytes($"PLAYER_ID:{id}"), $"PLAYER_ID:{id}".Length, sender);
                    udpServer.Send(Encoding.UTF8.GetBytes($"ROOM_ID:{room.RoomId}"), $"ROOM_ID:{room.RoomId}".Length, sender);

                    Console.WriteLine($"Player {id} assigned to room {room.RoomId}");
                }
                else if (msg.Contains(':'))
                {
                    string[] parts = msg.Split(':');
                    if (int.TryParse(parts[0], out int id) && parts.Length == 8)
                    {
                        Console.WriteLine($"Mise à jour des données du joueur {id}");
                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);
                        Animation anim = Enum.Parse<Animation>(parts[3]);
                        bool facing = bool.Parse(parts[4]);
                        float vertical = float.Parse(parts[5]);
                        float moveX = float.Parse(parts[6]);
                        float moveY = float.Parse(parts[7]);

                        lastSeen[id] = DateTime.UtcNow;
                        playerPositions[id] = new Vector2f(x, y);
                        playerAnimations[id] = anim;
                        playerFacings[id] = facing;
                        playerVerticalSpeeds[id] = vertical;
                        playerMovements[id] = new Vector2f(moveX, moveY);
                        Console.WriteLine($"Position mise à jour pour le joueur {id}: ({x}, {y})");
                    }
                }

                HandleDisconnections();
                BroadcastStates();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }
    }

    private GameRoom CreateRoom()
    {
        int id = rooms.Count + 1;
        GameRoom room = new(id);
        rooms[id] = room;
        Console.WriteLine($"Created new room: {id}");
        return room;
    }

    private void HandleDisconnections()
    {
        var toRemove = lastSeen.Where(kv => (DateTime.UtcNow - kv.Value).TotalSeconds > TimeoutSeconds)
                               .Select(kv => kv.Key)
                               .ToList();

        foreach (int id in toRemove)
        {
            if (playerToRoom.TryGetValue(id, out int roomId) && rooms.TryGetValue(roomId, out GameRoom? room))
                room.RemovePlayer(id);

            playerEndpoints.Remove(id);
            playerPositions.Remove(id);
            playerToRoom.Remove(id);
            lastSeen.Remove(id);

            Console.WriteLine($"Player {id} disconnected due to timeout.");
        }
    }

    private void BroadcastStates()
    {
        foreach (var room in rooms.Values)
        {
            if (room.Players.Count == 0) continue;

            Console.WriteLine($"Broadcast pour la salle {room.RoomId} - Nombre de joueurs: {room.Players.Count}");
            Console.WriteLine($"La salle est-elle prête? {room.IsReady}");

            StringBuilder sb = new();
            DataState roomState = new(room.IsReady ? DState.playing : DState.waiting);
            sb.Append(roomState.message);
            sb.Append('|');

            foreach (int id in room.Players)
            {
                if (!playerAnimations.ContainsKey(id) || !playerFacings.ContainsKey(id) || !playerVerticalSpeeds.ContainsKey(id)
                    || !playerMovements.ContainsKey(id) ||!playerPositions.ContainsKey(id) )
                {
                    Console.WriteLine($"Données manquantes pour le joueur {id}");
                    continue;
                }

                PlayerServerInfo Pinfo = new (playerPositions[id],playerAnimations[id],playerFacings[id],playerVerticalSpeeds[id],playerMovements[id]);
                PlayerMessage message = new(Pinfo, id);
                Console.WriteLine($"Ajout des données du joueur {id}: {message.Message}");
                sb.Append(message.Message);
                sb.Append('|');
            }

            string packet = sb.ToString().TrimEnd('|');
            Console.WriteLine($"Packet final: {packet}");
            byte[] bytes = Encoding.UTF8.GetBytes(packet);

            foreach (int id in room.Players)
            {
                if (playerEndpoints.TryGetValue(id, out var ep))
                {
                    udpServer.Send(bytes, bytes.Length, ep);
                    Console.WriteLine($"Packet envoyé au joueur {id}");
                }
                else
                {
                    Console.WriteLine($"Endpoint non trouvé pour le joueur {id}");
                }
            }
        }
    }
}
