using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gravity
{
    public class Level : IDisposable
    {
        public const int CellSize = 24;

        public readonly int Columns;
        public readonly int Rows;

        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;
        public Point Size => new(Width, Height);

        public readonly Cell[,] Cells;
        private readonly Texture2D cellTexture;
        private readonly Texture2D waterTexture;
        private readonly ContentManager content;
        private readonly bool showBounds = false;

        public Level(Texture2D levelMap, IServiceProvider serviceProvider)
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
            waterTexture = content.Load<Texture2D>("Textures/tile_0033");

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
                        var _ when pixel == Color.Blue => Cell.CellType.Water,
                        var _ when pixel == Color.Yellow => Cell.CellType.Spawn,
                        _ => throw new ArgumentException($"Grid cell color ({pixel}) not supported!"),
                    };

                    var cell = new Cell(x, y, type, type == Cell.CellType.Wall);
                    Cells[x, y] = cell;
                }
            }
        }

        public List<Point> GetSpawnPositions()
        {
            var positions = new List<Point>();
            foreach (var cell in Cells)
            {
                if (cell.Type == Cell.CellType.Spawn)
                    positions.Add(cell.Bounds.Center);
            }
            return positions;
        }

        public bool IsWithinBounds(int cx, int cy)
        {
            return cx >= 0 && cx < Columns && cy >= 0 && cy < Rows;
        }

        public bool HasCollision(int cx, int cy)
        {
            return !IsWithinBounds(cx, cy) || Cells[cx, cy].Solid;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var cell in Cells)
            {
                var texture = cell.Type switch
                {
                    Cell.CellType.Wall => cellTexture,
                    Cell.CellType.Water => waterTexture,
                    _ => null,
                };

                if (texture != null)
                    spriteBatch.Draw(texture, cell.Bounds, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

                if (showBounds)
                {
                    var color = cell.Solid ? Color.Red: Color.White;
                    var outline = cell.Bounds;
                    outline.Inflate(-1f, -1f);
                    spriteBatch.DrawRectangleOutline(outline, color, 1f);
                }
            }
        }

        public Cell this[int col, int row] => Cells[col, row];

        public void Dispose()
        {
            content.Unload();
        }
    }
}
