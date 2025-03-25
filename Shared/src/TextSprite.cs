using SFML.Graphics;
using SFML.System;

namespace Shared
{
    public class TextSprite
    {
        private Sprite sprite;
        private RenderTexture renderTexture;
        private Text text;
        private bool needsUpdate = true;

        public TextSprite(string text, Font font, uint characterSize)
        {
            renderTexture = new RenderTexture(640, 360);
            this.text = new Text(text, font, characterSize);
            this.text.FillColor = Color.White;
            sprite = new Sprite();
            UpdateSprite();
        }

        public void SetText(string newText)
        {
            if (text.DisplayedString != newText)
            {
                text.DisplayedString = newText;
                needsUpdate = true;
            }
        }

        public void SetPosition(Vector2f position)
        {
            sprite.Position = position;
        }

        private void UpdateSprite()
        {
            if (!needsUpdate) return;

            renderTexture.Clear(Color.Transparent);
            renderTexture.Draw(text);
            renderTexture.Display();
            sprite.Texture = renderTexture.Texture;
            needsUpdate = false;
        }

        public void Draw(RenderWindow window)
        {
            UpdateSprite();
            window.Draw(sprite);
        }
    }
} 