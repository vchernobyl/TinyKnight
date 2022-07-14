using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Level
    {
        #region Public
        public const int CellSize = 24;

        public readonly int Columns;
        public readonly int Rows;

        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;
        public Point Size => new(Width, Height);

        public readonly Cell[,] Cells;
        #endregion

        private readonly Texture2D cellTexture;
        private readonly bool showBounds = false;

        public Level(Texture2D levelMap, GameplayScreen gameplayScreen, ContentManager content)
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

            cellTexture = content.Load<Texture2D>("Textures/tile_0009");

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
                        var _ when pixel == Color.Black => Cell.CellType.Empty,
                        var _ when pixel == Color.White => Cell.CellType.Wall,
                        var _ when pixel == Color.Red => Cell.CellType.WalkerSpawn,
                        var _ when pixel == Color.Yellow  => Cell.CellType.FlyerSpawn,
                        var _ when pixel == Color.Blue => Cell.CellType.Hero,
                        _ => throw new ArgumentException($"Cell type with color {pixel} is not supported"),
                    };
                    
                    if (type == Cell.CellType.WalkerSpawn)
                    {
                        var spawner = new Portal(new Vector2(x * CellSize, y * CellSize), gameplayScreen, Portal.EnemyType.Walker);
                        gameplayScreen.AddEntity(spawner);
                    }
                    if (type == Cell.CellType.FlyerSpawn)
                    {
                        var spawner = new Portal(new Vector2(x * CellSize, y * CellSize), gameplayScreen, Portal.EnemyType.Flyer);
                        gameplayScreen.AddEntity(spawner);
                    }

                    var cell = new Cell(x, y, type, type == Cell.CellType.Wall);
                    Cells[x, y] = cell;
                }
            }
        }

        public bool IsWithinBounds(int cx, int cy)
        {
            return cx >= 0 && cx < Columns && cy >= 0 && cy < Rows;
        }

        public bool HasCollision(int cx, int cy)
        {
            return !IsWithinBounds(cx, cy) || Cells[cx, cy].Solid;
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (var cell in Cells)
            {
                if (cell.Type == Cell.CellType.Wall)
                    batch.Draw(cellTexture, cell.Bounds, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

                if (showBounds)
                {
                    var color = cell.Solid ? Color.Red: Color.White;
                    var outline = cell.Bounds;
                    outline.Inflate(-1f, -1f);
                    batch.DrawRectangleOutline(outline, color, 1f);
                }
            }
        }

        public Cell this[int col, int row] => Cells[col, row];
    }
}
