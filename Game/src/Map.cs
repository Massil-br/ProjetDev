using SFML.Graphics;
using SFML.System;

namespace src
{
    public class Map
    {
        private int width;
        private int height;
        private int[,] tiles;
        private RectangleShape tileShape;

        public Map(int width, int height, float tileSize)
        {
            this.width = width;
            this.height = height;
            tiles = new int[width, height];
            tileShape = new RectangleShape(new Vector2f(tileSize, tileSize));
            tileShape.FillColor = Color.White;

            // Initialiser la carte avec des tuiles par défaut (par exemple, 0 pour vide, 1 pour une tuile solide)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = 0; // Par défaut, toutes les tuiles sont vides
                }
            }
        }

        public void SetTile(int x, int y, int tileType)
        {
            int adjustedX = x + width / 2;
            int adjustedY = y + height / 2;

            if (adjustedX >= 0 && adjustedX < width && adjustedY >= 0 && adjustedY < height)
            {
                tiles[adjustedX, adjustedY] = tileType;
            }
        }

        public int IsColliding(FloatRect bounds)
        {
            int startX = (int)Math.Floor((bounds.Left + width * tileShape.Size.X / 2) / tileShape.Size.X);
            int startY = (int)Math.Floor((bounds.Top + height * tileShape.Size.Y / 2) / tileShape.Size.Y);
            int endX = (int)Math.Floor((bounds.Left + bounds.Width + width * tileShape.Size.X / 2) / tileShape.Size.X);
            int endY = (int)Math.Floor((bounds.Top + bounds.Height + height * tileShape.Size.Y / 2) / tileShape.Size.Y);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height && tiles[x, y] != 0)
                    {
                        return tiles[x, y];
                    }
                }
            }

            return 0; // Pas de collision
        }

        public void Draw(RenderWindow window)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y] != 0) // Si la tuile n'est pas vide
                    {
                        tileShape.FillColor = tiles[x, y] switch
                        {
                            1 => Color.White, // Tuile solide
                            2 => Color.Blue,  // Tuile d'eau
                            3 => Color.Red,   // Tuile de lave
                            // Ajoutez d'autres types de tuiles ici
                            _ => Color.White,
                        };
                        tileShape.Position = new Vector2f((x - width / 2) * tileShape.Size.X, (y - height / 2) * tileShape.Size.Y);
                        window.Draw(tileShape);
                    }
                }
            }
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public float GetTileSize()
        {
            return tileShape.Size.X; // Supposons que la largeur et la hauteur des tuiles sont égales
        }

        public void InitTestMap()
        {
            SetTile(0, 1, 1); 
            SetTile(1, 1, 1); 
            SetTile(2,1,2);
            SetTile(-1,1,2);
            SetTile(-1,-2,1);
            for (int i = -(width/2); i < width/2; i++){
                SetTile(i,24,3);
            }

            for (int i = 3;  i < 200; i++){
                SetTile(i, -4, 1);
            }
            SetTile(200,-4,2);
            
            for (int i = 202; i < width/2; i++){
                SetTile(i, -8,1);
            }

            SetTile(-24,-1,3);
        }
    }
}