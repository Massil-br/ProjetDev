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

        private Font mainFont = new("src/assets/Font/Poppins-ExtraBold.ttf");
        private uint baseFontSize = 30;
        private uint FontSize = 30;
        private Text PlayerHealthUi;

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

        private const float PlayerHealthUiPositionX = -0.48f;
        private const float PlayerHealthUiPositionY =-0.48f;

        private List<Projectile> projectiles = new();
        private float shootCooldown = 1f; // Temps entre chaque tir
        private float shootTimer = 0f;

        Text fpsText = new();
        Text playerPosition = new();
        Text playerVerticalSpeed = new();

        public Player(string texturePath, string name, int maxHealth, int attackDamage)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.isAlive = true;

            this.attackDamage = attackDamage;
            texture = TextureManager.GetTexture(texturePath);
            sprite = new Sprite(texture);
            sprite.Position = new Vector2f(0, -1); // Spawn au point (0,0)
            float scaleX = 32f / sprite.TextureRect.Width;
            float scaleY = 32f / sprite.TextureRect.Height;
            sprite.Scale = new Vector2f(Math.Abs(scaleX), scaleY); // S'assurer que le scale initial est positif
            
            // Définir l'origine au centre du sprite
            sprite.Origin = new Vector2f(sprite.TextureRect.Width / 2, sprite.TextureRect.Height / 2);
            PlayerHealthUi = new Text("", mainFont, FontSize);
            fpsText = new("FPS: " + frameCount * 2, mainFont, FontSize);
            playerPosition = new("Player position: " + sprite.Position.X + ", " + sprite.Position.Y, mainFont, FontSize);
            playerVerticalSpeed = new("Player Vertical Speed: " + verticalSpeed, mainFont, FontSize);

            IdleSpriteList =
            [
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
            ];

            RunSpriteList =
            [
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
            ];

            JumpSpriteList =
            [
                TextureManager.GetTexture("src/assets/Player/04-Jump/PS_BALD GUY_JumpUp_000.png"),
                TextureManager.GetTexture("src/assets/Player/04-Jump/PS_BALD GUY_JumpFall_000.png"),
            ];

            HurtSpriteList =
            [
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_000.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_001.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_002.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_003.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_004.png"),
                TextureManager.GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_005.png"),
            ];

            DeadSpriteList =
            [
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
            ];

            currentAnimation = IdleSpriteList;
        }

        public void Update(RenderWindow window, float deltaTime, Map map, Camera camera)
        {
            deltaTime = Math.Min(deltaTime, 0.2f);

            HandleInput(deltaTime, map);
            CheckHealth();
            ApplyGravity(deltaTime, map);
            HandleShooting(window, deltaTime, camera, map);
            UpdateProjectiles(deltaTime);
            UpdatePlayerUi(window);
            PlayerAnimation(deltaTime);
            Debug(deltaTime, window); 
            Render(window,camera  );
        }

        private void ApplyGravity(float deltaTime, Map map)
        {
            if (applyGravity)
            {
                verticalSpeed += gravity * deltaTime * 3;
                Vector2f newPosition = sprite.Position + new Vector2f(0, verticalSpeed * deltaTime);
                
                // Créer une zone de collision qui prend en compte l'origine centrée
                FloatRect newBounds = new FloatRect(
                    newPosition.X - (sprite.GetGlobalBounds().Width / 2) ,
                    newPosition.Y - (sprite.GetGlobalBounds().Height / 2) ,
                    sprite.GetGlobalBounds().Width -5,
                    sprite.GetGlobalBounds().Height- 4
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
                newPosition.X -( sprite.GetGlobalBounds().Width / 2) ,
                newPosition.Y - (sprite.GetGlobalBounds().Height / 2) ,
                sprite.GetGlobalBounds().Width -5,
                sprite.GetGlobalBounds().Height-4
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

        private uint CalculateFontSize(RenderWindow window)
        {
            // Calculer la taille de police en fonction de la taille de la fenêtre
            // On utilise la largeur de la fenêtre comme référence
            float baseWidth = 1920f; // Largeur de référence
            float currentWidth = window.Size.X;
            float scale = currentWidth / baseWidth;
            
            // Limiter la taille minimale et maximale
            uint newSize = (uint)(baseFontSize * scale);
            return Math.Max(12, Math.Min(40, newSize));
        }

        private void UpdateTextSize(RenderWindow window)
        {
            uint newSize = CalculateFontSize(window);
            if (newSize != FontSize)
            {
                FontSize = newSize;
                PlayerHealthUi.CharacterSize = FontSize;
                fpsText.CharacterSize = FontSize;
                playerPosition.CharacterSize = FontSize;
                playerVerticalSpeed.CharacterSize = FontSize;
            }
        }

        private void Debug(float deltaTime,  RenderWindow window)
        {   
            timer += deltaTime;
            frameCount++;

            if (timer >= 0.5f)
            {
                fpsText.DisplayedString = "FPS: " + frameCount * 2;
                playerPosition.DisplayedString = "Player position: " + sprite.Position.X + ", " + sprite.Position.Y;
                playerVerticalSpeed.DisplayedString = "Player Vertical Speed: " + verticalSpeed;
                timer = 0;
                frameCount = 0;
            }

            UpdateTextSize(window);

            // Utiliser la vue UI pour le texte
            View uiView = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            window.SetView(uiView);

            // Calculer la position en pixels
            float screenWidth = window.Size.X;
            float screenHeight = window.Size.Y;
            
            fpsText.Position = new Vector2f(
                screenWidth * 0.02f,
                screenHeight * 0.02f
            );
            fpsText.FillColor = Color.White;

            playerPosition.Position = new Vector2f(
                screenWidth * 0.02f,
                screenHeight * 0.05f
            );
            playerPosition.FillColor = Color.White;

            playerVerticalSpeed.Position = new Vector2f(
                screenWidth * 0.02f,
                screenHeight * 0.08f
            );
            playerVerticalSpeed.FillColor = Color.White;

            

            // Restaurer la vue du jeu
            
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

        private void HandleShooting(RenderWindow window, float deltaTime, Camera camera, Map map)
        {
            if (!isAlive) return;

            shootTimer -= deltaTime;
            if (shootTimer <= 0 && Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                // Obtenir la position de la souris dans le monde du jeu
                Vector2i mousePos = Mouse.GetPosition(window);
                Vector2f worldPos = window.MapPixelToCoords(mousePos, camera.GetView());
                
                // Créer un nouveau projectile avec la référence à la map
                Projectile projectile = new Projectile(sprite.Position, worldPos, map);
                projectiles.Add(projectile);
                
                shootTimer = shootCooldown;
            }
        }

        private void UpdateProjectiles(float deltaTime)
        {
            // Mettre à jour tous les projectiles actifs
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(deltaTime);
                
                // Supprimer les projectiles inactifs
                if (!projectiles[i].IsActive())
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public void Render(RenderWindow window, Camera camera)
        {   
            DrawPlayerUi(window, camera); 
            window.Draw(sprite);
            // Dessiner tous les projectiles
            foreach (var projectile in projectiles)
            {
                projectile.Draw(window);
            }
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



        private void UpdatePlayerUi(RenderWindow window){
            UpdateTextSize(window);

            // Utiliser la vue UI pour le texte
            View uiView = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            window.SetView(uiView);

            // Calculer la position en pixels
            float screenWidth = window.Size.X;
            float screenHeight = window.Size.Y;
            
            PlayerHealthUi.DisplayedString = health + "/" + maxHealth + " HP";
            PlayerHealthUi.Position = new Vector2f(
                screenWidth * 0.02f,
                screenHeight * 0.95f
            );
            PlayerHealthUi.FillColor = Color.White;
        } 

        private void DrawPlayerUi(RenderWindow window, Camera camera)
        {   
            window.Draw(fpsText);
            window.Draw(playerPosition);
            window.Draw(playerVerticalSpeed);
            window.Draw(PlayerHealthUi);
            window.SetView(camera.GetView());
        }

        
        
  
    }
}
