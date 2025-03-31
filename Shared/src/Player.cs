using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace Shared
{
    public class Player
    {
        // Player attributes
        private string name;
        private int health;
        private int maxHealth;
        private bool isAlive;
        private int attackDamage;
        private float speed = 100.0f * 1.3f;
        private float jumpForce = 300f;
        private FloatRect hitbox;

        // Shooting projectiles
        private List<Projectile> projectiles = new();
        private float shootCooldown = 0.5f;
        private float shootTimer = 0f;
        private bool projectileShot = false;
        private bool wasMousePressed = false;

        // Physics
        private float fallGracePeriod = 0.15f; // Time during which the player can jump after leaving the ground
        private float fallGraceTimer = 0f;
        private bool applyGravity = true;
        private float gravity = 9.8f * 15;
        private float verticalSpeed = 0.0f;

        // Player animation
        private Texture[] idleSpriteList;
        private Texture[] runSpriteList;
        private Texture[] jumpSpriteList;
        private Texture[] hurtSpriteList;
        private Texture[] deadSpriteList;

        private float animationTimer = 0f;
        private float animationSpeed = 0.1f; // Animation speed in seconds per frame
        private int currentFrame = 0;
        private Texture[] currentAnimation = [];
        private Texture[] previousAnimation = []; // To detect animation changes

        private int intAnimation = 0;

        private bool isFacingRight = true;

        // Rendering
        private Texture texture;
        private Sprite sprite;

        // Player UI
        private const float playerHealthUiPositionX = -0.48f;
        private const float playerHealthUiPositionY = -0.48f;
        private Font mainFont = new("src/assets/Font/Poppins-ExtraBold.ttf");
        private uint baseFontSize = 30;
        private uint fontSize = 30;
        private Text playerHealthUi;

        // Debug UI
        private float fpsTimer = 0;
        private int frameCount = 0;
        private Text fpsText = new();
        private Text playerPositionText = new();
        private Text playerVerticalSpeedText = new();
        private Vector2f playerPosition; // Player position

        public Player(string name, int maxHealth, int attackDamage)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.isAlive = true;
            this.attackDamage = attackDamage;

            texture = TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_000.png");
            sprite = new Sprite(texture);
            playerPosition = new Vector2f(0, -1); // Initial position
            float scaleX = 32f / sprite.TextureRect.Width;
            float scaleY = 32f / sprite.TextureRect.Height;
            sprite.Scale = new Vector2f(Math.Abs(scaleX), scaleY); // Ensure initial scale is positive

            // Set origin to the center of the sprite
            sprite.Origin = new Vector2f(sprite.TextureRect.Width / 2, sprite.TextureRect.Height / 2);
            playerHealthUi = new Text("", mainFont, fontSize);
            fpsText = new Text("FPS: " + frameCount * 2, mainFont, fontSize);
            playerPositionText = new Text("Player position: " + playerPosition.X + ", " + playerPosition.Y, mainFont, fontSize);
            playerVerticalSpeedText = new Text("Player Vertical Speed: " + verticalSpeed, mainFont, fontSize);

            idleSpriteList = new Texture[]
            {
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_000.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_001.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_002.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_003.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_004.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_005.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_006.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_007.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_008.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_009.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_010.png"),
                TextureManager.GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_011.png"),
            };

            runSpriteList = new Texture[]
            {
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_000.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_001.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_002.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_003.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_004.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_005.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_006.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_007.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_008.png"),
                TextureManager.GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_009.png"),
            };

            jumpSpriteList = new Texture[]
            {
                TextureManager.GetTexture("src/assets/Player/04-Jump/PS_BALD GUY_JumpUp_000.png"),
                TextureManager.GetTexture("src/assets/Player/04-Jump/PS_BALD GUY_JumpFall_000.png"),
            };

            hurtSpriteList = new Texture[]
            {
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_000.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_001.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_002.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_003.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_004.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_005.png"),
            };

            deadSpriteList = new Texture[]
            {
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_000.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_001.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_002.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_003.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_004.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_005.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_006.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_007.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_008.png"),
                TextureManager.GetTexture("src/assets/Player/07-Dead/Dead10.png"),
            };

            currentAnimation = idleSpriteList;
        }

        public void Update(RenderWindow window, float deltaTime, Map map, Camera camera)
        {
            deltaTime = Math.Min(deltaTime, 0.2f);

            HandleInput(deltaTime, map);
            CheckHealth();
            ApplyGravity(deltaTime, map);
            HandleShooting(window, deltaTime, camera, map);
            UpdateProjectiles(deltaTime);
            PlayerAnimation(deltaTime);
            
            Render(window, camera);
        }

        private void ApplyGravity(float deltaTime, Map map)
        {
            if (applyGravity)
            {
                verticalSpeed += gravity * deltaTime * 3;
                Vector2f newPosition = playerPosition + new Vector2f(0, verticalSpeed * deltaTime);

                // Update hitbox with reduced width of 10 pixels
                hitbox = new FloatRect(newPosition.X - 8, newPosition.Y - 12, 16, 24);

                int tileType = map.IsColliding(hitbox);
                if (tileType == 0 && newPosition.Y >= -map.GetHeight() * map.GetTileSize() / 2 && newPosition.Y + sprite.GetGlobalBounds().Height / 2 <= map.GetHeight() * map.GetTileSize() / 2)
                {
                    playerPosition = newPosition; // Update player position
                }
                else if (tileType == 3) // Collision with tile type 3 (lava)
                {
                    playerPosition = new Vector2f(1 * map.GetTileSize(), 0 * map.GetTileSize()); // Reset player position
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

            if (fallGraceTimer > 0)
            {
                fallGraceTimer -= deltaTime;
            }

            Vector2f movement = new(0, 0);

            // Handle jump
            if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && (TileOnGround(map) == 1 || fallGraceTimer > 0))
            {
                verticalSpeed = -jumpForce;  // Jump
                fallGraceTimer = 0; // Reset fall grace timer
            }

            // Handle left movement
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                movement.X -= speed * deltaTime;
                isFacingRight = false;
            }

            // Handle right movement
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                movement.X += speed * deltaTime;
                isFacingRight = true;
            }

            // Handle jump on tile type 2
            if (TileOnGround(map) == 2)
            {
                verticalSpeed = -jumpForce * 1.5f;
            }

            // Calculate new position
            Vector2f newPosition = playerPosition + movement;

            hitbox.Left = newPosition.X - (hitbox.Width / 2);
            hitbox.Top = newPosition.Y - (hitbox.Height / 2);

            // Check collisions
            int tileType = map.IsColliding(hitbox);
            if (tileType == 0 && newPosition.X >= -map.GetWidth() * map.GetTileSize() / 2 && newPosition.X + hitbox.Width / 2 <= map.GetWidth() * map.GetTileSize() / 2)
            {
                playerPosition = newPosition; // Update player position
            }

            // Update sprite orientation
            UpdateSpriteOrientation();
        }

        private int TileOnGround(Map map)
        {
            // Create a collision area that takes into account the centered origin
            FloatRect bounds = new FloatRect(
                playerPosition.X - 1, // Move the bounds to the center horizontally
                playerPosition.Y + sprite.GetGlobalBounds().Height / 2, // Move the bounds down to check collision with the ground
                2, // Reduce width to check only the center
                1  // Reduce height to check only the bottom
            );

            int tileType = map.IsColliding(bounds);
            if (tileType == 1)
            {
                fallGraceTimer = fallGracePeriod; // Reset fall grace timer
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

        private uint CalculateFontSize(RenderWindow window)
        {
            // Calculate font size based on window size
            // Use window width as reference
            float baseWidth = 1920f; // Reference width
            float currentWidth = window.Size.X;
            float scale = currentWidth / baseWidth;

            // Limit minimum and maximum size
            uint newSize = (uint)(baseFontSize * scale);
            return Math.Max(12, Math.Min(40, newSize));
        }

        private void UpdateTextSize(RenderWindow window)
        {
            uint newSize = CalculateFontSize(window);
            if (newSize != fontSize)
            {
                fontSize = newSize;
                playerHealthUi.CharacterSize = fontSize;
                fpsText.CharacterSize = fontSize;
                playerPositionText.CharacterSize = fontSize;
                playerVerticalSpeedText.CharacterSize = fontSize;
            }
        }

        public void Debug(float deltaTime, RenderWindow window)
        {
            fpsTimer += deltaTime;
            frameCount++;

            if (fpsTimer >= 0.5f)
            {
                fpsText.DisplayedString = "FPS: " + frameCount * 2;
                playerPositionText.DisplayedString = "Player position: " + playerPosition.X / 32 + ", " + playerPosition.Y / 32;
                playerVerticalSpeedText.DisplayedString = "Player Vertical Speed: " + verticalSpeed / 32;
                fpsTimer = 0;
                frameCount = 0;
            }

            UpdateTextSize(window);

            // Use UI view for text
            View uiView = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            window.SetView(uiView);

            // Calculate position in pixels
            float screenWidth = window.Size.X;
            float screenHeight = window.Size.Y;

            fpsText.Position = new Vector2f(screenWidth * 0.02f, screenHeight * 0.02f);
            fpsText.FillColor = Color.White;

            playerPositionText.Position = new Vector2f(screenWidth * 0.02f, screenHeight * 0.05f);
            playerPositionText.FillColor = Color.White;

            playerVerticalSpeedText.Position = new Vector2f(screenWidth * 0.02f, screenHeight * 0.08f);
            playerVerticalSpeedText.FillColor = Color.White;
        }

        private void PlayerAnimation(float deltaTime)
        {
            // Determine which animation to use
            if (!isAlive)
            {
                currentAnimation = deadSpriteList;
                intAnimation = 29;
            }
            else if (verticalSpeed != 0)
            {
                currentAnimation = jumpSpriteList;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Q) || Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                currentAnimation = runSpriteList;
                intAnimation = 11;
            }
            else
            {
                currentAnimation = idleSpriteList;
                intAnimation = 0;
            }

            // Reset frame if animation has changed
            if (currentAnimation != previousAnimation)
            {
                currentFrame = 0;
                previousAnimation = currentAnimation;

            }

            // Play appropriate animation
            if (currentAnimation == jumpSpriteList)
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
                if (currentAnimation == deadSpriteList)
                {
                    // For death animation, stop at the last frame
                    if (currentFrame < currentAnimation.Length - 1)
                    {
                        currentFrame++;
                    }
                }
                else
                {
                    // For other animations, loop
                    currentFrame = (currentFrame + 1) % currentAnimation.Length;
                }
            }
            sprite.Texture = currentAnimation[currentFrame];
            intAnimation += currentFrame;
        }

        private void PlayJumpAnimation()
        {
            if (jumpSpriteList == null || jumpSpriteList.Length < 2) return;

            if (verticalSpeed < 0)
            {
                sprite.Texture = jumpSpriteList[0];
                intAnimation = 21;
                
            }
            else if (verticalSpeed > 0)
            {
                sprite.Texture = jumpSpriteList[1];
                intAnimation = 22;
            }
        }

        private void HandleShooting(RenderWindow window, float deltaTime, Camera camera, Map map)
        {
            if (!isAlive) return;

            shootTimer -= deltaTime;
            bool isMousePressed = Mouse.IsButtonPressed(Mouse.Button.Left);
            if (shootTimer <= 0 && isMousePressed && !projectileShot)
            {
                projectileShot = true;
                // Get mouse position in game world
                Vector2i mousePos = Mouse.GetPosition(window);
                Vector2f worldPos = window.MapPixelToCoords(mousePos, camera.GetView());

                // Create a new projectile with reference to the map
                Projectile projectile = new Projectile(playerPosition, worldPos, map);
                projectiles.Add(projectile);

                shootTimer = shootCooldown;
            }
            if (!isMousePressed && wasMousePressed)
            {
                projectileShot = false;
            }

            wasMousePressed = isMousePressed;
        }

        private void UpdateProjectiles(float deltaTime)
        {
            // Update all active projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(deltaTime);

                // Remove inactive projectiles
                if (!projectiles[i].IsActive())
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public void Render(RenderWindow window, Camera camera)
        {
            
            float offsetX = isFacingRight ? -5 : 5; // Offset to the right if facing right, otherwise to the left
            sprite.Position = new Vector2f(playerPosition.X + offsetX, playerPosition.Y); // Add 4 for vertical offset
            window.Draw(sprite);
            // Draw all projectiles
            foreach (var projectile in projectiles)
            {
                projectile.Draw(window);
            }
        }

        public float GetVerticalSpeed(){
            return this.verticalSpeed;
            }
        public void SetVerticalSpeed(float verticalSpeed){
            this.verticalSpeed = verticalSpeed;
        }
        public Texture GetSpriteTexture()
        {
            return sprite.Texture;
        }

        public string GetName()
        {
            return name;
        }

        private void UpdateSpriteOrientation()
        {
            if (!isFacingRight)
            {
                sprite.Scale = new Vector2f(Math.Abs(sprite.Scale.X), sprite.Scale.Y);
            }
            else
            {
                sprite.Scale = new Vector2f(-Math.Abs(sprite.Scale.X), sprite.Scale.Y);
            }
        }

        public void UpdatePlayerUi(RenderWindow window)
        {
            UpdateTextSize(window);

            // Use UI view for text
            View uiView = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            window.SetView(uiView);

            // Calculate position in pixels
            float screenWidth = window.Size.X;
            float screenHeight = window.Size.Y;

            playerHealthUi.DisplayedString = health + "/" + maxHealth + " HP";
            playerHealthUi.Position = new Vector2f(screenWidth * 0.02f, screenHeight * 0.95f);
            playerHealthUi.FillColor = Color.White;
        }

        public  void DrawPlayerUi(RenderWindow window, Camera camera)
        {
            window.Draw(fpsText);
            window.Draw(playerPositionText);
            window.Draw(playerVerticalSpeedText);
            window.Draw(playerHealthUi);
            window.SetView(camera.GetView());
        }

        public Vector2f GetPosition()
        {
            return playerPosition;
        }

        public int GetIntAnimation(){
            return intAnimation;
        }

        public void SetSpriteTexture(int intAnimation){
            sprite.Texture = TextureManager.textures[intAnimation];
        }

        // Méthode pour mettre à jour la position du joueur
        public void UpdatePosition(Vector2f newPosition)
        {
            playerPosition = newPosition;
        }
    }
}
