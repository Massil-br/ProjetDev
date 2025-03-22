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
        public static Text titleText = new Text("Main Menu", mainFont, 30);
        public static Text soloText = new Text("Solo", mainFont, 20);
        public static Text multiplayerText = new Text("Multiplayer", mainFont, 20);
        public static Text quitText = new Text("Quit", mainFont, 20);
        public static int selectedOption = 0;

        public static float ArrowSelectionTimer = 0f;

        private const float TitlePositionY = -0.20f;
        private const float SoloPositionY = 0f;
        private const float MultiplayerPositionY = 0.15f;
        private const float QuitPositionY = 0.30f;

        public static void InitMainMenu(Camera camera)
        {
            float viewWidth = camera.GetView().Size.X;
            float viewHeight = camera.GetView().Size.Y;
            Vector2f cameraPosition = camera.GetView().Center;

            // Positionner et colorer le texte du titre
            titleText.Position = new Vector2f(
                cameraPosition.X - titleText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * TitlePositionY) - titleText.GetGlobalBounds().Height / 2
            );
            titleText.FillColor = Color.White;

            // Positionner et colorer le texte de l'option Solo
            soloText.Position = new Vector2f(
                cameraPosition.X - soloText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * SoloPositionY) - soloText.GetGlobalBounds().Height / 2
            );
            soloText.FillColor = Color.White;

            // Positionner et colorer le texte de l'option Multijoueur
            multiplayerText.Position = new Vector2f(
                cameraPosition.X - multiplayerText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * MultiplayerPositionY) - multiplayerText.GetGlobalBounds().Height / 2
            );
            multiplayerText.FillColor = Color.White;

            // Positionner et colorer le texte de l'option Quitter
            quitText.Position = new Vector2f(
                cameraPosition.X - quitText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * QuitPositionY) - quitText.GetGlobalBounds().Height / 2
            );
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
                InitMainMenu(camera);
                MainMenuInitialized = true;
            }

            // S'assurer que la vue de la caméra est appliquée avant de dessiner
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

            // Mettre à jour les positions en fonction de la position de la caméra
            float viewHeight = camera.GetView().Size.Y;
            Vector2f cameraPosition = camera.GetView().Center;

            // Mettre à jour les positions des textes
            titleText.Position = new Vector2f(
                cameraPosition.X - titleText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * TitlePositionY) - titleText.GetGlobalBounds().Height / 2
            );

            soloText.Position = new Vector2f(
                cameraPosition.X - soloText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * SoloPositionY) - soloText.GetGlobalBounds().Height / 2
            );

            multiplayerText.Position = new Vector2f(
                cameraPosition.X - multiplayerText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * MultiplayerPositionY) - multiplayerText.GetGlobalBounds().Height / 2
            );

            quitText.Position = new Vector2f(
                cameraPosition.X - quitText.GetGlobalBounds().Width / 2,
                cameraPosition.Y + (viewHeight * QuitPositionY) - quitText.GetGlobalBounds().Height / 2
            );

            // Draw menu
            window.Draw(titleText);
            window.Draw(soloText);
            window.Draw(multiplayerText);
            window.Draw(quitText);

            return State.MainMenu;
        }
    }
}



