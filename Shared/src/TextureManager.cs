using SFML.Graphics;
using System.Collections.Generic;

namespace Shared
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture> textureCache = new();

        public static Texture GetTexture(string path)
        {
            if (!textureCache.ContainsKey(path))
            {
                textureCache[path] = new Texture(path);
            }
            return textureCache[path];
        }

        public static void ClearCache()
        {
            foreach (var texture in textureCache.Values)
            {
                texture.Dispose();
            }
            textureCache.Clear();
        }
    }
} 