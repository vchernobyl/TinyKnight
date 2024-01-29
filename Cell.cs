using Microsoft.Xna.Framework;

namespace TinyKnight
{
    public class Cell
    {
        public enum CellType
        {
            Empty,
            Wall, 
            WalkerSpawn, 
            FlyerSpawn, 
            Hero, 
        }

        public readonly int X;
        public readonly int Y;

        public CellType Type { get; set; }
        public bool Solid { get; set; }

        public Rectangle Bounds => new Rectangle(X * Level.CellSize, Y * Level.CellSize, Level.CellSize, Level.CellSize);
        public Point Location => new Point(X, Y);
        public Vector2 Position => new Vector2(X * Level.CellSize, Y * Level.CellSize);

        public Cell(int x, int y, CellType type, bool solid)
        {
            X = x;
            Y = y;
            Type = type;
            Solid = solid;
        }
    }
}
