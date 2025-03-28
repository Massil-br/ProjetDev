using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;

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

    class Loop
    {
        private RenderWindow window;
        private Clock clock;
        private Camera camera;
        private State currentState;
        private State lastState = State.MainMenu;

        private Player? player;

        private bool firstEntryToGameLoop = true;
        private bool firstEntryToMainMenu = true;
        private Map? map;

        public Loop()
        {
            window = new RenderWindow(new VideoMode(1280, 720), "Ma fenêtre SFML");
            clock = new Clock();
            camera = new Camera(640, 360);
           

            window.Closed += (sender, e) => window.Close();
            window.Resized += OnWindowResized;
            window.LostFocus += OnLostFocus;
            window.GainedFocus += OnGainedFocus;
        }

        private void OnGainedFocus(object? sender, EventArgs e)
        {
             currentState = lastState;
        }

        private void OnLostFocus(object? sender, EventArgs e)
        {
            lastState = currentState;
            currentState = State.Paused;
        }

        private void OnWindowResized(object? sender, SizeEventArgs e)
        {   
            GameLoop.ResizeCamera(camera);
            Camera.ViewHeight = camera.GetHeight();
            Camera.ViewWidth = camera.GetWidth();
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
                        if (firstEntryToMainMenu){
                            MainMenu.InitMainMenu();
                            firstEntryToMainMenu  = false;
                            firstEntryToGameLoop = true;
                        }
                        currentState = MainMenu.RunMainMenu(window, deltaTime);
                        break;
                    case State.Playing:
                        if (firstEntryToGameLoop){
                            firstEntryToGameLoop = false;
                            firstEntryToMainMenu = true;
                            GameLoop.ResizeCamera(camera);
                        }
                        currentState = GameLoop.RunGameLoop(player, map, deltaTime, window, camera);
                        break;
                    case State.Paused:
                        // Handle Paused state
                        break;
                    case State.GameOver:
                        // Handle GameOver state
                        break;
                }

                window.Display();
            }
        }
    }
}