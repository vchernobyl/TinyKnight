using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TinyKnight.AI
{
    public interface IWeightedGraph<T>
    {
        int Cost(Point a, Point b);
        IEnumerable<Point> Neighbours(Point p);
    }
}
