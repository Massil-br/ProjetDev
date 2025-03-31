using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SFML.System;

public class Server
{
    private UdpClient udpServer;
    private IPEndPoint clientEndPoint;
    private const int Port = 12345;
    private int nextPlayerId = 1;
    private Dictionary<int, Vector2f> players = new Dictionary<int, Vector2f>();
    private Dictionary<int, IPEndPoint> playerEndPoints = new Dictionary<int, IPEndPoint>();

    public Server()
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

                string[] parts = message.Split(':');
                if (parts[0] == "REQUEST_ID")
                {
                    int assignedId = nextPlayerId++;
                    byte[] idData = Encoding.UTF8.GetBytes(assignedId.ToString());
                    udpServer.Send(idData, idData.Length, clientEndPoint);
                    playerEndPoints[assignedId] = clientEndPoint;
                    Console.WriteLine($"New player connected with ID {assignedId}");
                }
                else if (parts.Length == 3)
                {
                    if (int.TryParse(parts[0], out int playerId))
                    {
                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);
                        players[playerId] = new Vector2f(x, y);
                    }
                }

                // Build a message with all positions
                StringBuilder responseBuilder = new StringBuilder();
                foreach (var kvp in players)
                {
                    responseBuilder.Append($"{kvp.Key}:{kvp.Value.X}:{kvp.Value.Y}|");
                }

                string responseMessage = responseBuilder.ToString().TrimEnd('|');
                byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);

                // Send positions to all clients
                foreach (var endPoint in playerEndPoints.Values)
                {
                    udpServer.Send(responseData, responseData.Length, endPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public void Stop()
    {
        udpServer.Close();
    }
}
