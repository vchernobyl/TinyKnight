using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Gravity
{
    public class Level : IDisposable
    {
        public const int CellSize = 24;

        public readonly int Columns;
        public readonly int Rows;
        public readonly Cell[,] Cells;

        private readonly Texture2D cellTexture;
        private readonly Texture2D goalTexture;
        private readonly ContentManager content;
        private readonly Hero hero;

        private bool showBounds = false;

        public Level(Texture2D levelMap, LevelManager levelManager, IServiceProvider serviceProvider)
        {
            // Get texture pixels into a one-dimensional array.
            var pixels = new Color[levelMap.Width * levelMap.Height];
            levelMap.GetData(pixels);

            // Store pixels into a two-dimensional array.
            var levelData = new Color[levelMap.Width, levelMap.Height];
            for (int y = 0; y < levelMap.Height; y++)
            {
                for (int x = 0; x < levelMap.Width; x++)
                {
                    levelData[x, y] = pixels[x + y * levelMap.Width];
                }
            }

            content = new ContentManager(serviceProvider, rootDirectory: "Content");
            cellTexture = content.Load<Texture2D>("Textures/tile_0009");
            goalTexture = content.Load<Texture2D>("Textures/tile_0111");

            Columns = levelMap.Width;
            Rows = levelMap.Height;
            Cells = new Cell[Columns, Rows];

            // Generate level out of the image data.
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    var pixel = levelData[x, y];
                    var type = pixel switch
                    {
                        var color when color == Color.Black => Cell.CellType.Empty,
                        var color when color == Color.White => Cell.CellType.Wall,
                        var color when color == Color.Yellow => Cell.CellType.Goal,
                        _ => throw new InvalidOperationException($"Grid cell color ({pixel}) not supported!"),
                    };
                    Cells[x, y] = new Cell(x, y, type, type == Cell.CellType.Wall);
                }
            }

            hero = new Hero(content.Load<Texture2D>("Textures/character_0000"), this);
            hero.OnLevelCompleted += () => { levelManager.NextLevel(); };
            hero.SetCoordinates(50f, 50f);
        }

        public void Update(GameTime gameTime)
        {
            hero.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var cell in Cells)
            {
                var texture = cell.Type switch
                {
                    Cell.CellType.Wall => cellTexture,
                    Cell.CellType.Goal => goalTexture,
                    _ => null,
                };

                if (texture != null)
                    spriteBatch.Draw(texture, cell.Bounds, Color.White);

                if (showBounds)
                {
                    var outline = cell.Bounds;
                    outline.Inflate(-1f, -1f);
                    spriteBatch.DrawRectangleOutline(outline, Color.White, 1f);
                }
            }

            hero.Draw(spriteBatch);
        }

        public void Dispose()
        {
            content.Unload();
        }
    }
}
