using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace Shared
{
    public class Player
    {
        private string name;
        private int health;
        private int maxHealth;
        private bool isAlive;
        private int attackDamage;
        private float speed = 100.0f;
        private Sprite sprite;

        private bool applyGravity = true;
        private float gravity = 9.8f * 10;
        private float verticalSpeed = 0.0f;

        private float timer = 0;
        private int frameCount = 0;

        public Player(string texturePath, string name, int maxHealth, int attackDamage)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.isAlive = true;
            
            this.attackDamage = attackDamage;
            Texture texture = new(texturePath);
            sprite = new Sprite(texture);
            sprite.Position = new Vector2f(0, -1); // Spawn au point (0,0)
            
        }

        public void Update(RenderWindow window, float deltaTime, Map map)
        {   
            deltaTime = Math.Min(deltaTime, 0.2f);

            HandleInput(deltaTime, map);
            CheckHealth();
            ApplyGravity(deltaTime, map);
            Debug(deltaTime);
            Render(window);
        }

        private void ApplyGravity(float deltaTime, Map map)
        {
            if (applyGravity)
            {
                verticalSpeed += gravity * deltaTime * 3;
                Vector2f newPosition = sprite.Position + new Vector2f(0, verticalSpeed * deltaTime);
                FloatRect newBounds = sprite.GetGlobalBounds();
                newBounds.Top = newPosition.Y;

                int tileType = map.IsColliding(newBounds);
                if (tileType == 0 && newPosition.Y >= -map.GetHeight() * map.GetTileSize() / 2 && newPosition.Y + sprite.GetGlobalBounds().Height <= map.GetHeight() * map.GetTileSize() / 2)
                {
                    sprite.Position = newPosition;
                }
                else if (tileType == 3) // Collision avec une tuile de type 3 (lave)
                {
                    sprite.Position = new Vector2f(0, -1 * map.GetTileSize()); // Réinitialise la position du joueur
                    verticalSpeed = 0.0f;
                }
                 else
                {
                     verticalSpeed = 0.0f;
                 }
            }
        }

        private void HandleInput(float deltaTime, Map map)
        {
            Vector2f movement = new(0, 0);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && TileOnGround(map) != 0 && TileOnGround(map) != 3)
            {
                verticalSpeed = -speed * 1.5f; // Sauter
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                movement.X -= speed * deltaTime;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                movement.X += speed * deltaTime;
            }

            if (TileOnGround(map) == 2)
            {
                verticalSpeed = -speed * 3;
            }

            Vector2f newPosition = sprite.Position + movement;
            FloatRect newBounds = sprite.GetGlobalBounds();
            newBounds.Left = newPosition.X;

            int tileType = map.IsColliding(newBounds);
            if (tileType == 0 && newPosition.X >= -map.GetWidth() * map.GetTileSize() / 2 && newPosition.X + sprite.GetGlobalBounds().Width <= map.GetWidth() * map.GetTileSize() / 2)
            {
                sprite.Position = newPosition;
            }
        }

        private int TileOnGround(Map map)
        {
            FloatRect bounds = sprite.GetGlobalBounds();
            bounds.Left += bounds.Width / 2 - 1; // Déplace les limites vers le centre horizontalement
            bounds.Top += bounds.Height; // Déplace les limites vers le bas pour vérifier la collision avec le sol
            bounds.Width = 2; // Réduire la largeur pour ne vérifier que le centre
            bounds.Height = 1; // Réduire la hauteur pour ne vérifier que le bas

            return map.IsColliding(bounds);
        }
        private void CheckHealth()
        {
            if (isAlive)
            {
                if (health <= 0)
                {
                    health = 0;
                    isAlive = false;
                }
                else if (health > maxHealth)
                {
                    health = maxHealth;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (isAlive)
            {
                health -= damage;
            }
        }

        private void Debug(float deltaTime)
        {
            timer += deltaTime;
            frameCount++;

            if (timer >= 0.5f)
            {   
                Console.Clear();
                Console.WriteLine("FPS: " + frameCount *2);
                Console.WriteLine("Player position: " + sprite.Position.X + ", " + sprite.Position.Y);
                
               
                timer = 0;
                frameCount = 0;
            }
        }

        public void Render(RenderWindow window)
        {
            window.Draw(sprite);
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        public string GetName()
        {
            return name;
        }
    }
}
