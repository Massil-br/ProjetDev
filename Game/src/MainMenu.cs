using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

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

        private const float TitlePositionY = 0.25f;
        private const float SoloPositionY = 0.5f;
        private const float MultiplayerPositionY = 0.6f;
        private const float QuitPositionY = 0.7f;

        public static void InitMainMenu(RenderWindow window, Camera camera)
        {
            FloatRect viewRect = camera.GetView().Viewport;
            float viewWidth = camera.GetView().Size.X;
            float viewHeight = camera.GetView().Size.Y;

            // Positionner et colorer le texte du titre
            titleText.Position = new Vector2f(viewWidth / 2 - titleText.GetGlobalBounds().Width / 2, viewHeight * TitlePositionY);
            titleText.FillColor = Color.White;

            // Positionner et colorer le texte de l'option Solo
            soloText.Position = new Vector2f(viewWidth / 2 - soloText.GetGlobalBounds().Width / 2, viewHeight * SoloPositionY);
            soloText.FillColor = Color.White;

            // Positionner et colorer le texte de l'option Multijoueur
            multiplayerText.Position = new Vector2f(viewWidth / 2 - multiplayerText.GetGlobalBounds().Width / 2, viewHeight * MultiplayerPositionY);
            multiplayerText.FillColor = Color.White;

            // Positionner et colorer le texte de l'option Quitter
            quitText.Position = new Vector2f(viewWidth / 2 - quitText.GetGlobalBounds().Width / 2, viewHeight * QuitPositionY);
            quitText.FillColor = Color.White;

            MainMenuInitialized = true;
        }

        public static State RunMainMenu(RenderWindow window, float deltaTime, Camera camera)
        {
            if (ArrowSelectionTimer > 0)
            {
                ArrowSelectionTimer -= deltaTime;
            }
            if (!MainMenuInitialized)
            {
                InitMainMenu(window, camera);
                MainMenuInitialized = true;
            }

            window.SetView(camera.GetView());

            // Handle input
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) && ArrowSelectionTimer <= 0)
            {
                selectedOption = (selectedOption - 1 + 3) % 3;
                ArrowSelectionTimer = 0.2f;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) && ArrowSelectionTimer <= 0)
            {
                selectedOption = (selectedOption + 1) % 3;
                ArrowSelectionTimer = 0.2f;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
            {
                switch (selectedOption)
                {
                    case 0:
                        return State.Playing;
                    case 1:
                        // Handle multiplayer option
                        break;
                    case 2:
                        window.Close();
                        break;
                }
            }

            // Update colors based on selection
            soloText.FillColor = selectedOption == 0 ? Color.Red : Color.White;
            multiplayerText.FillColor = selectedOption == 1 ? Color.Red : Color.White;
            quitText.FillColor = selectedOption == 2 ? Color.Red : Color.White;

            // Draw menu
            window.Draw(titleText);
            window.Draw(soloText);
            window.Draw(multiplayerText);
            window.Draw(quitText);

            return State.MainMenu;
        }
    }
}



