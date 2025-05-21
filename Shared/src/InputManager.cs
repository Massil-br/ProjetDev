using SFML.Window;

namespace Shared
{
    public static class InputManager
    {
        private static Dictionary<Keyboard.Key, bool> keyStates = new Dictionary<Keyboard.Key, bool>();
        private static Dictionary<Keyboard.Key, bool> previousKeyStates = new Dictionary<Keyboard.Key, bool>();

        public static void Update()
        {
            // Sauvegarde l'état précédent des touches
            previousKeyStates = new Dictionary<Keyboard.Key, bool>(keyStates);

            // Met à jour l'état actuel des touches
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            {
                keyStates[key] = Keyboard.IsKeyPressed(key);
            }
        }

        public static bool IsKeyPressed(Keyboard.Key key)
        {
            return keyStates.ContainsKey(key) && keyStates[key] && 
                   (!previousKeyStates.ContainsKey(key) || !previousKeyStates[key]);
        }

        public static bool IsKeyDown(Keyboard.Key key)
        {
            return keyStates.ContainsKey(key) && keyStates[key];
        }

        public static bool IsKeyReleased(Keyboard.Key key)
        {
            return previousKeyStates.ContainsKey(key) && previousKeyStates[key] && 
                   (!keyStates.ContainsKey(key) || !keyStates[key]);
        }
    }
} 