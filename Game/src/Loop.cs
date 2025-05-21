using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace src
{
    public enum State
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Multiplayer,
        Auth
    }

    public class Loop
    {
        const string ServerIp = "127.0.0.1";
        const int Port = 11111;
        

        private RenderWindow window;
        private Clock clock;
        private Camera camera;
        private State currentState;
        private State lastState = State.MainMenu;

 

        private Player? player;
        private bool firstEntryToGameLoop = true;
        private bool firstEntryToMainMenu = true;

        private Map? map;
        Thread? clientThread;

        private float Timer;

        private Client? udpClient;
        private Dictionary<int, Player> otherPlayers = new Dictionary<int, Player>();

        public Loop()
        {
           
            window = new RenderWindow(new VideoMode(1280, 720), "My SFML Window");
            clock = new Clock();
            camera = new Camera(640, 360);

            window.Closed += (sender, e) => window.Close();
            window.Resized += OnWindowResized;
            window.LostFocus += OnLostFocus;
            window.GainedFocus += OnGainedFocus;
        }

        private void OnWindowResized(object? sender, SizeEventArgs e)
        {
            
            GameLoop.ResizeCamera(camera);
            Camera.ViewHeight = camera.GetHeight();
            Camera.ViewWidth = camera.GetWidth();
        }

        private void OnLostFocus(object? sender, EventArgs e)
        {
            lastState = currentState;
            player?.SetPause(true);
            currentState = State.Paused;
            window.SetFramerateLimit(30);
        }

        private void OnGainedFocus(object? sender, EventArgs e)
        {
            player?.SetPause(false);
            currentState = lastState;
            window.SetFramerateLimit(5000);
        }

        public void Run()
        {
           

            (player, map) = GameLoop.InitGameLoop();
            currentState = State.MainMenu;

            while (window.IsOpen)
            {
                float deltaTime = clock.Restart().AsSeconds();
                window.DispatchEvents();
                window.Clear();
                Timer += deltaTime;

               

                switch (currentState)
                {
                    case State.MainMenu:
                        if (firstEntryToMainMenu)
                        {
                            MainMenu.InitMainMenu();
                            firstEntryToMainMenu = false;
                            firstEntryToGameLoop = true;
                        }
                        currentState = MainMenu.RunMainMenu(window, deltaTime);
                        break;

                    

                    case State.Playing:
                        if (firstEntryToGameLoop)
                        {
                            firstEntryToGameLoop = false;
                            firstEntryToMainMenu = true;
                            
                            GameLoop.ResizeCamera(camera);
                        }
                        currentState = GameLoop.RunGameLoop(player, map, deltaTime, window, camera, otherPlayers, currentState);
                        break;

                    case State.Multiplayer:
                        if (firstEntryToGameLoop)
                        {
                            firstEntryToGameLoop = false;
                            firstEntryToMainMenu = true;
                           
                            GameLoop.ResizeCamera(camera);


                            udpClient = new Client(ServerIp, Port);
                            bool connected = udpClient.StartMatchmaking(player, timeoutSeconds: 5);

                            if (!connected)
                            {
                                Console.WriteLine("Connection to server timed out.");
                                currentState = State.MainMenu;
                                break;
                            }

                            clientThread = new Thread(() => udpClient.Start(player));
                            clientThread.IsBackground = true;
                            clientThread.Start();
                        }

                            otherPlayers = udpClient?.GetOtherPlayers() ?? new Dictionary<int, Player>();
                            currentState = GameLoop.RunGameLoop(player, map, deltaTime, window, camera, otherPlayers, currentState);
                        break;

                    case State.Paused:
                        currentState = GameLoop.RunGameLoop(player, map, deltaTime, window, camera, otherPlayers, currentState);
                        break;
                }
                if (Timer >= 1 || currentState!= lastState){
                    Console.WriteLine(currentState);
                    Timer= 0;
                }

                window.Display();
            }

            clientThread?.Interrupt();
        }
    }
}
