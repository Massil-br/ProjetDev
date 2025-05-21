using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;
using System;

namespace src
{
    public static class AuthScreen
    {
        private static bool isInitialized = false;
        private static bool isAuthenticated = false;

        public static void InitAuthScreen()
        {
            isInitialized = true;
            isAuthenticated = false;
        }

        public static State RunAuthScreen(RenderWindow window, float deltaTime)
        {
            if (!isInitialized)
            {
                InitAuthScreen();
            }

            // On reste sur l'interface web tant que l'authentification n'est pas faite
            if (!isAuthenticated)
            {
                return State.Auth;
            }

            return State.Multiplayer;
        }

        public static bool IsAuthenticated()
        {
            return isAuthenticated;
        }

        public static void SetAuthenticated(bool value)
        {
            isAuthenticated = value;
        }
    }
} 