using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Gravity.AI
{
    public class NavigationGrid : IWeightedGraph<Point>
    {
        public static readonly Point[] Directions =
        {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1),
        };

        public readonly int Width;
        public readonly int Height;
        public readonly ISet<Point> Solids;
        public readonly ISet<Point> NearSolids;

        public NavigationGrid(int width, int height)
        {
            Width = width;
            Height = height;
            Solids = new HashSet<Point>();
            NearSolids = new HashSet<Point>();
        }

        public bool InBounds(Point p)
        {
            return 0 <= p.X && p.X < Width &&
                0 <= p.Y && p.Y < Height;
        }

        public bool Passable(Point p)
        {
            return !Solids.Contains(p);
        }

        public int Cost(Point a, Point b)
        {
            return 1;// NearSolids.Contains(a) ? 2 : 1;
        }

        public IEnumerable<Point> Neighbours(Point p)
        {
            foreach (var direction in Directions)
            {
                var next = new Point(p.X + direction.X, p.Y + direction.Y);
                if (InBounds(next) && Passable(next))
                    yield return next;
            }
        }
    }
}
