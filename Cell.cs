using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Cell
    {
        public enum CellType
        {
            Empty,
            Wall, 
            Water,
            Spawn,
        }

        public readonly int X;
        public readonly int Y;

        public CellType Type { get; set; }
        public bool Solid { get; set; }

        public Rectangle Bounds => new(X * Level.CellSize, Y * Level.CellSize, Level.CellSize, Level.CellSize);

        public Cell(int x, int y, CellType type, bool solid)
        {
            X = x;
            Y = y;
            Type = type;
            Solid = solid;
        }
    }
}
