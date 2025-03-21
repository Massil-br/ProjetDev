
using SFML.Graphics;
using Shared;

namespace src
{
    public class GameLoop
    {   

        public static bool CameraResized = false;
        public static bool GameInitialized = false;

        public static (Player player, Map map) InitGameLoop()
        {   
            //"src/assets/barrel.png"
            var player = new Player("src/assets/Player/01-Idle/PS_BALD GUY_Idle_000.png", "Player", 100, 10);
            var map = new Map(500, 50, 32); // Exemple de taille de carte et taille de tuile
            map.InitTestMap();
            GameInitialized = true;
            return (player, map);
        }

        public static  void ResizeCamera(Camera camera){
            //camera.Resize((int)Math.Floor(camera.GetWidth() * 0.4), (int)Math.Floor(camera.GetHeight() * 0.4));
            camera.Resize(640,360);
        }

        public static State RunGameLoop(Player player, Map map, float deltaTime, RenderWindow window, Camera camera)
        {   
            
            
            player.Update(window, deltaTime, map);
            camera.Update(player.GetSprite().Position);
            map.Draw(window);
            window.SetView(camera.GetView());

            return State.Playing;
        }

        
        
        

        // public static State RunMultiplayerLoop(){
        //     Ip serverIp = new ("127.0.0.1", 8994);
        //     Client client = new (serverIp);
        // }


    }
}
