using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;

namespace src
{
    public class MainMenu
    {
        public static bool MainMenuInitialized = false;
        public static Font mainFont = new("src/assets/Team 401.ttf");
        public static Text titleText = new Text("Main Menu", mainFont, 50);
        public static Text soloText = new Text("Solo", mainFont, 30);
        public static Text multiplayerText = new Text("Multiplayer", mainFont, 30);
        public static Text quitText = new Text("Quit", mainFont, 30);
        public static int selectedOption = 0;

        public static float ArrowSelectionTimer = 0f;

        private static float mouseClickTimer;
        private static float mouseClickCooldown =0.5f;

        private const float TitlePositionY = -0.20f;
        private const float SoloPositionY = 0f;
        private const float MultiplayerPositionY = 0.15f;
        private const float QuitPositionY = 0.30f;

        private static State state = State.MainMenu;

        public static void InitMainMenu()
        {   
            state = State.MainMenu;
            // Initialisation des couleurs
            titleText.FillColor = Color.White;
            soloText.FillColor = Color.White;
            multiplayerText.FillColor = Color.White;
            quitText.FillColor = Color.White;

            MainMenuInitialized = true;
        }

        public static State RunMainMenu(RenderWindow window, float deltaTime)
        {   
            View uiView = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            window.SetView(uiView);
            if (ArrowSelectionTimer > 0)
            {
                ArrowSelectionTimer -= deltaTime;
            }

            if (mouseClickTimer > 0){
                mouseClickTimer -= deltaTime;
            }
            if (!MainMenuInitialized)
            {
                InitMainMenu();
            }
            
            HandleKeyboardInput();
            HandleMouseInput(window);

            // Update colors based on selection
            soloText.FillColor = selectedOption == 0 ? Color.Red : Color.White;
            multiplayerText.FillColor = selectedOption == 1 ? Color.Red : Color.White;
            quitText.FillColor = selectedOption == 2 ? Color.Red : Color.White;

            // Mettre à jour les positions des textes par rapport au centre de la fenêtre
            Vector2f windowCenter = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);

            titleText.Position = new Vector2f(
                windowCenter.X - titleText.GetGlobalBounds().Width / 2,
                windowCenter.Y + (window.Size.Y * TitlePositionY) - titleText.GetGlobalBounds().Height / 2
            );

            soloText.Position = new Vector2f(
                windowCenter.X - soloText.GetGlobalBounds().Width / 2,
                windowCenter.Y + (window.Size.Y * SoloPositionY) - soloText.GetGlobalBounds().Height / 2
            );

            multiplayerText.Position = new Vector2f(
                windowCenter.X - multiplayerText.GetGlobalBounds().Width / 2,
                windowCenter.Y + (window.Size.Y * MultiplayerPositionY) - multiplayerText.GetGlobalBounds().Height / 2
            );

            quitText.Position = new Vector2f(
                windowCenter.X - quitText.GetGlobalBounds().Width / 2,
                windowCenter.Y + (window.Size.Y * QuitPositionY) - quitText.GetGlobalBounds().Height / 2
            );

            // Draw menu
            window.Draw(titleText);
            window.Draw(soloText);
            window.Draw(multiplayerText);
            window.Draw(quitText);

            return state;
        }


        private static void HandleKeyboardInput()
        {
            if ((Keyboard.IsKeyPressed(Keyboard.Key.Z)|| Keyboard.IsKeyPressed(Keyboard.Key.Up)) && ArrowSelectionTimer <= 0)
            {
                selectedOption = (selectedOption - 1 + 3) % 3;
                ArrowSelectionTimer = 0.2f;
            }
            if ((Keyboard.IsKeyPressed(Keyboard.Key.S)|| Keyboard.IsKeyPressed(Keyboard.Key.Down)) && ArrowSelectionTimer <= 0)
            {
                selectedOption = (selectedOption + 1) % 3;
                ArrowSelectionTimer = 0.2f;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
            {
                ExecuteSelectedOption();
            }
        }


        private static void HandleMouseInput(RenderWindow window)
        {
            Vector2i mousePosition = Mouse.GetPosition(window);
            Vector2f worldMousePosition = window.MapPixelToCoords(mousePosition);

            // Check if the mouse is over any menu item
            if (soloText.GetGlobalBounds().Contains(worldMousePosition.X, worldMousePosition.Y))
            {
                selectedOption = 0;
                if (Mouse.IsButtonPressed(Mouse.Button.Left) && mouseClickTimer <=0)
                {
                    mouseClickTimer = mouseClickCooldown;
                    ExecuteSelectedOption();
                }
            }
            else if (multiplayerText.GetGlobalBounds().Contains(worldMousePosition.X, worldMousePosition.Y))
            {
                selectedOption = 1;
                if (Mouse.IsButtonPressed(Mouse.Button.Left)&& mouseClickTimer <=0)
                {
                    mouseClickTimer = mouseClickCooldown;
                    ExecuteSelectedOption();
                }
            }
            else if (quitText.GetGlobalBounds().Contains(worldMousePosition.X, worldMousePosition.Y))
            {
                selectedOption = 2;
                if (Mouse.IsButtonPressed(Mouse.Button.Left)&& mouseClickTimer <=0)
                {
                    mouseClickTimer = mouseClickCooldown;
                    ExecuteSelectedOption();
                }
            }
        }


        private static void  ExecuteSelectedOption()
        {
            switch (selectedOption)
            {
                case 0:
                    state =  State.Playing;
                    break;
                case 1:
                    Console.WriteLine("Multiplayer not implemented yet.");
                    state = State.MainMenu;
                    break;
                case 2:
                    Environment.Exit(0);
                     // Ne sera jamais atteint, mais requis par le compilateur
                    break;
                default:
                    state =State.MainMenu ;
                    break;
            }
        }

    }
}



