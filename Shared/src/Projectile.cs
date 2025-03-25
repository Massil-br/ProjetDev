using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Shared
{
    public class Projectile
    {
        private Sprite sprite;
        private Vector2f direction;
        private float speed = 500f;
        private bool isActive = true;
        private Map map;
        private float collisionDelay = 0.05f; // Délai avant l'activation des collisions
        private float collisionTimer = 0f; // Timer pour le délai

        private static readonly string texturePath = "src/assets/pixelFireBallWBackground-removebg-preview.png";

        public Projectile(Vector2f startPosition, Vector2f targetPosition, Map map)
        {
            this.map = map;
            Texture texture = TextureManager.GetTexture(texturePath);
            sprite = new Sprite(texture);
            sprite.Position = startPosition;
            
            // Redimensionner le sprite à 32x32 pixels
            float scaleX = 32f / sprite.TextureRect.Width;
            float scaleY = 32f / sprite.TextureRect.Height;
            sprite.Scale = new Vector2f(scaleX, scaleY);
            
            // Définir l'origine au centre du sprite
            sprite.Origin = new Vector2f(sprite.TextureRect.Width / 2, sprite.TextureRect.Height / 2);
            
            // Calculer la direction vers la cible
            direction = targetPosition - startPosition;
            float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            direction = new Vector2f(direction.X / length, direction.Y / length);
            
            // Ajuster la rotation du sprite pour qu'il pointe dans la direction du mouvement
            float angle = (float)(Math.Atan2(direction.Y, direction.X) * 180 / Math.PI);
            sprite.Rotation = angle;
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;
            
            // Mettre à jour le timer de collision
            collisionTimer += deltaTime;
            
            // Calculer la nouvelle position
            Vector2f newPosition = sprite.Position + direction * speed * deltaTime;
            
            // Créer une zone de collision qui prend en compte l'origine centrée
            FloatRect newBounds = new FloatRect(
                newPosition.X - (sprite.GetGlobalBounds().Width / 2) + 16,
                newPosition.Y - (sprite.GetGlobalBounds().Height / 2) + 16,
                sprite.GetGlobalBounds().Width - 32,
                sprite.GetGlobalBounds().Height - 32
            );

            // Vérifier les collisions avec la map seulement après le délai
            if (collisionTimer >= collisionDelay)
            {
                int tileType = map.IsColliding(newBounds);
                if (tileType != 0) // Si collision avec une tuile
                {
                    isActive = false;
                    return;
                }
            }

            // Vérifier si le projectile est hors des limites de la map
            if (newPosition.X < -map.GetWidth() * map.GetTileSize() / 2 || 
                newPosition.X > map.GetWidth() * map.GetTileSize() / 2 ||
                newPosition.Y < -map.GetHeight() * map.GetTileSize() / 2 || 
                newPosition.Y > map.GetHeight() * map.GetTileSize() / 2)
            {
                isActive = false;
                return;
            }

            // Si pas de collision, mettre à jour la position
            sprite.Position = newPosition;
        }

        public void Draw(RenderWindow window)
        {
            if (!isActive) return;
            window.Draw(sprite);
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void Deactivate()
        {
            isActive = false;
        }

        public FloatRect GetBounds()
        {
            return sprite.GetGlobalBounds();
        }
    }
}