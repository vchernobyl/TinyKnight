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
        private static readonly Point[] Directions =
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

    public class Pathfinding
    {
        private readonly Dictionary<Point, Point> cameFrom = new();
        private readonly Dictionary<Point, int> costSoFar = new();
        private readonly List<Point> path = new();
        private readonly IWeightedGraph<Point> graph;

        public Pathfinding(IWeightedGraph<Point> graph)
        {
            this.graph = graph;
        }

        private static int Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private static List<Point> SimplifyPath(List<Point> path)
        {
            var simplified = new List<Point>();
            var directionOld = Point.Zero;

            for (int i = 1; i < path.Count; i++)
            {
                var directionNew = path[i - 1] - path[i];
                if (directionNew != directionOld)
                    simplified.Add(path[i]);
                directionOld = directionNew;
            }

            return simplified;
        }

        public List<Point> FindPath(Point start, Point goal)
        {
            cameFrom.Clear();
            costSoFar.Clear();
            path.Clear();

            var frontier = new PriorityQueue<Point, int>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                // Reconstruct path and terminate.
                if (current == goal)
                {
                    while (current != start)
                    {
                        path.Add(current);
                        current = cameFrom[current];
                    }
                    path.Add(start);
                    path.Reverse();
                    break;
                }

                foreach (var next in graph.Neighbours(current))
                {
                    var newCost = costSoFar[current] + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            return SimplifyPath(path);
        }
    }
}
