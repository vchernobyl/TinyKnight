using TinyKnight.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight
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

        public bool CheckLineIsBlocked(Vector2 start, Vector2 end)
        {
            var startPoint = new Point(
                (int)start.X / CellSize, 
                (int)start.Y / CellSize);

            var endPoint = new Point(
                (int)end.X / CellSize,
                (int)end.Y / CellSize);

            var line = Bresenham.GetLine(startPoint, endPoint);
            foreach (var point in line)
            {
                foreach (var cell in Cells)
                {
                    if (cell.Solid && cell.Location == point)
                    {
                        DebugRenderer.AddLine(start, cell.Position, Color.Red);
                        return true;
                    }
                }
            }
            DebugRenderer.AddLine(start, end, Color.Green);
            return false;
        }

        public void Draw(SpriteBatch batch)
        {
            foreach (var cell in Cells)
            {
                if (cell.Type == Cell.CellType.Wall)
                {
                    var dest = new Rectangle(cell.Bounds.X, cell.Bounds.Y + 1, cell.Bounds.Width, cell.Bounds.Height);
                    batch.Draw(cellTexture, dest, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }

                if (cell.Solid && DebugInfo.ShowSolids)
                {
                    var outline = cell.Bounds;
                    DebugRenderer.AddRectangle(outline, Color.Red, .5f);
                }

                if (DebugInfo.ShowGrid)
                {
                    DebugRenderer.AddRectangle(cell.Bounds, Color.White, .5f);
                }
            }
        }
    }
}
