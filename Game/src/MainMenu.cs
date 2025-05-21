
using System;
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
        public static Text multiplayerTitleText = new Text("Multiplayer", mainFont, 50);
        public static Text soloText = new Text("Solo", mainFont, 30);
        public static Text multiplayerText = new Text("Multiplayer", mainFont, 30);
        public static Text quitText = new Text("Quit", mainFont, 30);
        public static Text hostGameText = new Text("Host Game", mainFont, 30);
        public static Text joinGameText = new Text("Join Game", mainFont, 30);
        public static Text backText = new Text("Back", mainFont, 30);


        public static int selectedOption = 0;

        private static float ArrowSelectionTimer = 0f;
        private static float mouseClickTimer;
        private static float mouseClickCooldown = 0.5f;

        private const float TitlePositionY = -0.20f;
        private const float SoloPositionY = 0f;
        private const float MultiplayerPositionY = 0.15f;
        private const float QuitPositionY = 0.30f;
        private const float HostGameY = 0f;
        private const float JoinGameY = 0.15f;
        private const float BackY = 0.30f;


        private static State state = State.MainMenu;
        private static View uiView = new View();
        private enum MenuState { Main, Multiplayer }
        private static MenuState menuState = MenuState.Main;


        public static void InitMainMenu()
        {
            state = State.MainMenu;
            menuState = MenuState.Main;

            titleText.FillColor = Color.White;
            soloText.FillColor = Color.White;
            multiplayerText.FillColor = Color.White;
            quitText.FillColor = Color.White;

            hostGameText.FillColor = Color.White;
            joinGameText.FillColor = Color.White;
            backText.FillColor = Color.White;

            MainMenuInitialized = true;
        }

        public static State RunMainMenu(RenderWindow window, float deltaTime)
        {
            uiView.Size = new Vector2f(window.Size.X, window.Size.Y);
            uiView.Center = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
            window.SetView(uiView);

            mouseClickTimer += deltaTime;
            Vector2f center = uiView.Center;
            float width = uiView.Size.X;
            float height = uiView.Size.Y;


            if (menuState == MenuState.Main)
            {
                titleText.Position = new Vector2f(center.X - titleText.GetGlobalBounds().Width / 2f, center.Y + height * TitlePositionY);
                soloText.Position = new Vector2f(center.X - soloText.GetGlobalBounds().Width / 2f, center.Y + height * SoloPositionY);
                multiplayerText.Position = new Vector2f(center.X - multiplayerText.GetGlobalBounds().Width / 2f, center.Y + height * MultiplayerPositionY);
                quitText.Position = new Vector2f(center.X - quitText.GetGlobalBounds().Width / 2f, center.Y + height * QuitPositionY);

                // Reset all colors
                soloText.FillColor = Color.White;
                multiplayerText.FillColor = Color.White;
                quitText.FillColor = Color.White;

                // Souris
                Vector2i mousePos = Mouse.GetPosition(window);
                Vector2f worldPos = window.MapPixelToCoords(mousePos);

                if (soloText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    soloText.FillColor = Color.Red;
                else if (multiplayerText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    multiplayerText.FillColor = Color.Red;
                else if (quitText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    quitText.FillColor = Color.Red;

                // Dessin
                window.Draw(titleText);
                window.Draw(soloText);
                window.Draw(multiplayerText);
                window.Draw(quitText);

                if (Mouse.IsButtonPressed(Mouse.Button.Left) && mouseClickTimer > mouseClickCooldown)
                {
                    mouseClickTimer = 0f;

                    if (soloText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                        state = State.Playing;
                    else if (multiplayerText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                        menuState = MenuState.Multiplayer;
                    else if (quitText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                        window.Close();
                }
            }


            else if (menuState == MenuState.Multiplayer)
            {

                multiplayerTitleText.Position = new Vector2f(center.X - multiplayerTitleText.GetGlobalBounds().Width / 2f, center.Y + height * TitlePositionY);
                multiplayerTitleText.FillColor = Color.White;
                window.Draw(multiplayerTitleText);

                hostGameText.Position = new Vector2f(center.X - hostGameText.GetGlobalBounds().Width / 2f, center.Y + height * HostGameY);
                joinGameText.Position = new Vector2f(center.X - joinGameText.GetGlobalBounds().Width / 2f, center.Y + height * JoinGameY);
                backText.Position = new Vector2f(center.X - backText.GetGlobalBounds().Width / 2f, center.Y + height * BackY);

                // Reset colors
                hostGameText.FillColor = Color.White;
                joinGameText.FillColor = Color.White;
                backText.FillColor = Color.White;

                Vector2i mousePos = Mouse.GetPosition(window);
                Vector2f worldPos = window.MapPixelToCoords(mousePos);

                if (hostGameText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    hostGameText.FillColor = Color.Red;
                else if (joinGameText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    joinGameText.FillColor = Color.Red;
                else if (backText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                    backText.FillColor = Color.Red;

                window.Draw(hostGameText);
                window.Draw(joinGameText);
                window.Draw(backText);


                

                if (Mouse.IsButtonPressed(Mouse.Button.Left) && mouseClickTimer > mouseClickCooldown)
                {
                    mouseClickTimer = 0f;

                    if (backText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                        menuState = MenuState.Main;
                    else if (hostGameText.GetGlobalBounds().Contains(worldPos.X, worldPos.Y))
                        state = State.Multiplayer;
                }
            }


            return state ;
        }

    }
}
