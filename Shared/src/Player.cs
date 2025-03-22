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
        private float speed = 100.0f * 1.3f;
        private Sprite sprite;

        private float JumpForce = 300f;

        private Texture[] IdleSpriteList;
        private Texture[] RunSpriteList;
        private Texture[] JumpSpriteList;
        private Texture[] HurtSpriteList;
        private Texture[] DeadSpriteList;

        private Texture texture;

        private float FallGracePeriod = 0.15f; // Temps pendant lequel le joueur peut sauter après avoir quitté le sol
        private float FallGraceTimer = 0f;

        private bool applyGravity = true;
        private float gravity = 9.8f * 15;
        private float verticalSpeed = 0.0f;

        private float timer = 0;
        private int frameCount = 0;

        private float animationTimer = 0f;
        private float animationSpeed = 0.1f; // Vitesse de l'animation en secondes par frame
        private int currentFrame = 0;
        private Texture[] currentAnimation = [];
        private Texture[] previousAnimation = []; // Pour détecter les changements d'animation
        
        private bool isFacingRight = true; // Nouvelle variable pour suivre la direction

        public Player(string texturePath, string name, int maxHealth, int attackDamage)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.isAlive = true;

            this.attackDamage = attackDamage;
            texture = new(texturePath);
            sprite = new Sprite(texture);
            sprite.Position = new Vector2f(0, -1); // Spawn au point (0,0)
            float scaleX = 32f / sprite.TextureRect.Width;
            float scaleY = 32f / sprite.TextureRect.Height;
            sprite.Scale = new Vector2f(Math.Abs(scaleX), scaleY); // S'assurer que le scale initial est positif
            
            // Définir l'origine au centre du sprite
            sprite.Origin = new Vector2f(sprite.TextureRect.Width / 2, sprite.TextureRect.Height / 2);

            IdleSpriteList =
            [
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_000.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_001.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_002.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_003.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_004.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_005.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_006.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_007.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_008.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_009.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_010.png"),
                new Texture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_011.png"),
            ];

            RunSpriteList =
            [
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_000.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_001.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_002.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_003.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_004.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_005.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_006.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_007.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_008.png"),
                new Texture("src/assets/Player/02-Run/PS_BALD GUY_Run_009.png"),
            ];

            JumpSpriteList =
            [
                new Texture("src/assets/Player/04-Jump/PS_BALD GUY_JumpUp_000.png"),
                new Texture("src/assets/Player/04-Jump/PS_BALD GUY_JumpFall_000.png"),
            ];

            HurtSpriteList =
            [
                new Texture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_000.png"),
                new Texture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_001.png"),
                new Texture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_002.png"),
                new Texture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_003.png"),
                new Texture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_004.png"),
                new Texture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_005.png"),
            ];

            DeadSpriteList =
            [
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_000.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_001.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_002.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_003.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_004.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_005.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_006.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_007.png"),
                new Texture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_008.png"),
                new Texture("src/assets/Player/07-Dead/Dead10.png"),
            ];

            currentAnimation = IdleSpriteList;
        }

        public void Update(RenderWindow window, float deltaTime, Map map)
        {
            deltaTime = Math.Min(deltaTime, 0.2f);

            HandleInput(deltaTime, map);
            CheckHealth();
            ApplyGravity(deltaTime, map);
            Debug(deltaTime);
            PlayerAnimation(deltaTime);
            Render(window);
        }

        private void ApplyGravity(float deltaTime, Map map)
        {
            if (applyGravity)
            {
                verticalSpeed += gravity * deltaTime * 3;
                Vector2f newPosition = sprite.Position + new Vector2f(0, verticalSpeed * deltaTime);
                
                // Créer une zone de collision qui prend en compte l'origine centrée
                FloatRect newBounds = new FloatRect(
                    newPosition.X - sprite.GetGlobalBounds().Width / 2,
                    newPosition.Y - sprite.GetGlobalBounds().Height / 2,
                    sprite.GetGlobalBounds().Width,
                    sprite.GetGlobalBounds().Height
                );

                int tileType = map.IsColliding(newBounds);
                if (tileType == 0 && newPosition.Y >= -map.GetHeight() * map.GetTileSize() / 2 && newPosition.Y + sprite.GetGlobalBounds().Height / 2 <= map.GetHeight() * map.GetTileSize() / 2)
                {
                    sprite.Position = newPosition;
                }
                else if (tileType == 3) // Collision avec une tuile de type 3 (lave)
                {
                    sprite.Position = new Vector2f(1*map.GetTileSize(), 0 * map.GetTileSize()); // Réinitialise la position du joueur
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
            if (!isAlive) return;
           
            if (FallGraceTimer > 0)
            {
                FallGraceTimer -= deltaTime;
            }

            Vector2f movement = new(0, 0);

            // Gérer le saut
            if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && (TileOnGround(map) != 0 || FallGraceTimer > 0) )
            {
                verticalSpeed = -JumpForce;  // Sauter
                FallGraceTimer = 0; // Réinitialiser le timer de grâce de chute
            }

            // Gérer le mouvement vers la gauche
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                movement.X -= speed * deltaTime;
                isFacingRight = false;
            }

            // Gérer le mouvement vers la droite
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                movement.X += speed * deltaTime;
                isFacingRight = true;
            }

            // Gérer le saut sur une tuile de type 2
            if (TileOnGround(map) == 2)
            {
                verticalSpeed = -JumpForce * 1.5f;
            }

            // Calculer la nouvelle position
            Vector2f newPosition = sprite.Position + movement;
            
            // Créer une zone de collision qui prend en compte l'origine centrée
            FloatRect newBounds = new FloatRect(
                newPosition.X - sprite.GetGlobalBounds().Width / 2,
                newPosition.Y - sprite.GetGlobalBounds().Height / 2,
                sprite.GetGlobalBounds().Width,
                sprite.GetGlobalBounds().Height
            );

            // Vérifier les collisions horizontales
            int tileType = map.IsColliding(newBounds);
            if (tileType == 0 && newPosition.X >= -map.GetWidth() * map.GetTileSize() / 2 && newPosition.X + sprite.GetGlobalBounds().Width / 2 <= map.GetWidth() * map.GetTileSize() / 2)
            {
                sprite.Position = newPosition;
            }

            // Mettre à jour l'orientation du sprite
            UpdateSpriteOrientation();
        }

        private int TileOnGround(Map map)
        {
            // Créer une zone de collision qui prend en compte l'origine centrée
            FloatRect bounds = new FloatRect(
                sprite.Position.X - 1, // Déplace les limites vers le centre horizontalement
                sprite.Position.Y + sprite.GetGlobalBounds().Height / 2, // Déplace les limites vers le bas pour vérifier la collision avec le sol
                2, // Réduire la largeur pour ne vérifier que le centre
                1  // Réduire la hauteur pour ne vérifier que le bas
            );

            int tileType = map.IsColliding(bounds);
            if (tileType != 0)
            {
                FallGraceTimer = FallGracePeriod; // Réinitialiser le timer de grâce de chute
            }

            return tileType;
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
                Console.WriteLine("Player Vertical Speed: " + verticalSpeed);

                timer = 0;
                frameCount = 0;
            }
        }

        private void PlayerAnimation(float deltaTime)
        {
            // Déterminer quelle animation utiliser
            if (!isAlive)
            {
                currentAnimation = DeadSpriteList;
            }
            else if (verticalSpeed != 0)
            {
                currentAnimation = JumpSpriteList;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Q) || Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                currentAnimation = RunSpriteList;
            }
            else
            {
                currentAnimation = IdleSpriteList;
            }

            // Réinitialiser la frame si l'animation a changé
            if (currentAnimation != previousAnimation)
            {
                currentFrame = 0;
                previousAnimation = currentAnimation;
            }

            // Jouer l'animation appropriée
            if (currentAnimation == JumpSpriteList)
            {
                PlayJumpAnimation();
            }
            else if (currentAnimation != null && currentAnimation.Length > 0)
            {
                PlayFrameAnimation(deltaTime);
            }
        }

        private void PlayFrameAnimation(float deltaTime)
        {
            animationTimer += deltaTime;
            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                if (currentAnimation == DeadSpriteList)
                {
                    // Pour l'animation de mort, on s'arrête à la dernière frame
                    if (currentFrame < currentAnimation.Length - 1)
                    {
                        currentFrame++;
                    }
                }
                else
                {
                    // Pour les autres animations, on boucle
                    currentFrame = (currentFrame + 1) % currentAnimation.Length;
                }
            }
            sprite.Texture = currentAnimation[currentFrame];
        }

        private void PlayJumpAnimation()
        {
            if (JumpSpriteList == null || JumpSpriteList.Length < 2) return;

            if (verticalSpeed < 0)
            {
                sprite.Texture = JumpSpriteList[0];
            }
            else if (verticalSpeed > 0)
            {
                sprite.Texture = JumpSpriteList[1];
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

        private void UpdateSpriteOrientation()
        {
            // Sauvegarder la position actuelle
            Vector2f currentPos = sprite.Position;
            
            if (!isFacingRight)
            {
                sprite.Scale = new Vector2f(Math.Abs(sprite.Scale.X), sprite.Scale.Y);
            }
            else
            {
                sprite.Scale = new Vector2f(-Math.Abs(sprite.Scale.X), sprite.Scale.Y);
            }

            // Restaurer la position
            sprite.Position = currentPos;
        }



        
    }
}
