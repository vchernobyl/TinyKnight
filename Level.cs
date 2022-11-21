using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Level
    {
        public const int CellSize = 8;

        public readonly int Columns;
        public readonly int Rows;

        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;
        public Point Size => new Point(Width, Height);

        public readonly Cell[,] Cells;

        private readonly Texture2D cellTexture;

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

        public bool CheckLineIsBlocked(Point start, Point end)
        {
            var line = Bresenham.GetLine(start, end);
            foreach (var point in line)
            {
                foreach (var cell in Cells)
                {
                    if (cell.Solid && cell.Location == point)
                    {
                        var a = new Vector2(start.X * CellSize + CellSize / 2f,
                            start.Y * CellSize + CellSize / 2f);
                        var b = new Vector2(cell.Position.X + CellSize / 2f,
                            cell.Position.Y + CellSize / 2f);
                        DebugRenderer.AddLine(a, b, Color.Red);
                        return true;
                    }
                }
            }
            var c = new Vector2(start.X * CellSize + CellSize / 2f,
                start.Y * CellSize + CellSize / 2f);
            var d = new Vector2(end.X * CellSize + CellSize / 2f,
                end.Y * CellSize + CellSize / 2f);
            DebugRenderer.AddLine(c, d, Color.Green);
            return false;
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (var cell in Cells)
            {
                if (cell.Type == Cell.CellType.Wall)
                    batch.Draw(cellTexture, cell.Bounds, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                if (cell.Solid && DebugInfo.ShowSolids)
                {
                    var outline = cell.Bounds;
                    batch.DrawRectangleOutline(outline, Color.Red, .5f);
                }

                if (DebugInfo.ShowGrid)
                {
                    batch.DrawRectangleOutline(cell.Bounds, Color.White, .5f);
                }
            }
        }
    }
}
