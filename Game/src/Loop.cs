using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace src
{
    public enum State
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Multiplayer
    }

    public class Loop
    {   
        const string ServerIp = "127.0.0.1";
        private RenderWindow window;
        private Clock clock;
        private Camera camera;
        private State currentState;
        private State lastState = State.MainMenu;

        private Player? player;
        private bool firstEntryToGameLoop = true;
        private bool firstEntryToMainMenu = true;
        private Map? map;

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
            currentState = State.Paused;
        }

        private void OnGainedFocus(object? sender, EventArgs e)
        {
            currentState = lastState;
        }

        public void Run()
        {
            if (!GameLoop.GameInitialized || player == null || map == null)
            {
                (player, map) = GameLoop.InitGameLoop();
            }

            currentState = State.MainMenu;
            while (window.IsOpen)
            {
                float deltaTime = clock.Restart().AsSeconds();
                window.DispatchEvents();
                window.Clear();

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
                        currentState = GameLoop.RunGameLoop(player, map, deltaTime, window, camera, otherPlayers);
                        break;

                    case State.Multiplayer:
                        if (firstEntryToGameLoop)
                        {
                            firstEntryToGameLoop = false;
                            firstEntryToMainMenu = false;
                            GameLoop.ResizeCamera(camera);
                            udpClient = new Client(ServerIp);
                            Thread clientThread = new Thread(() => udpClient.Start(player));
                            clientThread.IsBackground = true;
                            clientThread.Start();
                        }
                        // Update other players from the client
                        otherPlayers = udpClient?.GetOtherPlayers() ?? new Dictionary<int, Player>();
                        currentState = GameLoop.RunGameLoop(player, map, deltaTime, window, camera, otherPlayers);
                        break;
                }

                window.Display();
            }
        }
    }
}
