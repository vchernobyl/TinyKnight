using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Level
    {
        public const int CellSize = 24;

        public readonly int Columns;
        public readonly int Rows;

        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;
        public Point Size => new Point(Width, Height);

        public readonly Cell[,] Cells;

        private readonly Texture2D cellTexture;
        private readonly bool showBounds = false;

        public Level(int width, int height, Texture2D texture)
        {
            Columns = width;
            Rows = height;
            Cells = new Cell[Columns, Rows];
            cellTexture = texture;
        }

        public bool IsWithinBounds(int cx, int cy)
        {
            return cx >= 0 && cx < Columns && cy >= 0 && cy < Rows;
        }

        public bool HasCollision(int cx, int cy)
        {
            if (!IsWithinBounds(cx, cy))
                return false;
            return Cells[cx, cy].Solid;
        }

        public Cell this[int col, int row] => Cells[col, row];

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
    }
}
