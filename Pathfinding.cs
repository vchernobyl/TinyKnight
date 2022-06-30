using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Gravity
{
    public interface IWeightedGraph<T>
    {
        int Cost(Point a, Point b);
        IEnumerable<Point> Neighbours(Point p);
    }

    // TODO: This is VERY similar to the Level class.
    // It's probably even better to get rid of this class
    // altogether and just pass Level to the A* algorithm.
    public class SquareGrid : IWeightedGraph<Point>
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

        public SquareGrid(int width, int height)
        {
            Width = width;
            Height = height;
            Solids = new HashSet<Point>();
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
            // For now every passable node has the same cost.
            return 1;
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

    public class AStarSearch
    {
        public Dictionary<Point, Point> cameFrom = new();
        public Dictionary<Point, int> costSoFar = new();

        public static int Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public AStarSearch(IWeightedGraph<Point> graph, Point start, Point goal)
        {
            var path = new PriorityQueue<Point, int>();
            path.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (path.Count > 0)
            {
                var current = path.Dequeue();
                if (current.Equals(goal))
                    break;

                foreach (var next in graph.Neighbours(current))
                {
                    var newCost = costSoFar[current] + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + Heuristic(next, goal);
                        path.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }
        }
    }
}
