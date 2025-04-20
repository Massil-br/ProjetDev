using SFML.Graphics;
using Shared;
using System.Collections.Generic;

namespace src
{
    public class GameLoop
    {
        public static bool GameInitialized = false;

        public static (Player player, Map map) InitGameLoop()
        {
            var player = new Player("Player", 100, 10);
            var map = new Map(500, 50, 32);
            map.InitTestMap();
            GameInitialized = true;
            return (player, map);
        }

        public static void ResizeCamera(Camera camera)
        {
            camera.Resize(640, 360);
        }

        public static State RunGameLoop(Player player, Map map, float deltaTime, RenderWindow window, Camera camera, Dictionary<int, Player> otherPlayers, State state)
        {   
         
            player.Update(window, deltaTime, map, camera);
            player.UpdatePlayerUi(window);
            player.Debug(deltaTime, window);
            player.DrawPlayerUi(window, camera);
            camera.Update(player.GetPosition());
            map.Draw(window);
             
            window.SetView(camera.GetView());
           

            // Draw other players
            foreach (var otherPlayer in otherPlayers.Values)
            {
                otherPlayer.Render(window, deltaTime);
            }
            
            // Draw the main player
            player.Render(window, deltaTime);
           

            return state;  
        }



    }
}
