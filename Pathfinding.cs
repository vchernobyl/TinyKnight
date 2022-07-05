using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gravity
{
    public interface IWeightedGraph<T>
    {
        int Cost(Point a, Point b);
        IEnumerable<Point> Neighbours(Point p);
    }

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
            return NearSolids.Contains(a) ? 5 : 1;
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

        public void Draw(SpriteBatch batch)
        {
            foreach (var node in Solids)
            {
                var rect = new Rectangle(
                    node.X * Level.CellSize,
                    node.Y * Level.CellSize,
                    Level.CellSize,
                    Level.CellSize);
                batch.DrawRectangleOutline(rect, Color.Red, 2f);
            }

            foreach (var node in NearSolids)
            {
                var rect = new Rectangle(
                    node.X * Level.CellSize,
                    node.Y * Level.CellSize,
                    Level.CellSize,
                    Level.CellSize);
                batch.DrawRectangleOutline(rect, Color.Green, 2f);
            }
        }
    }

    public class Pathfinding
    {
        private readonly Dictionary<Point, Point> cameFrom = new();
        private readonly Dictionary<Point, int> costSoFar = new();
        private readonly List<Vector2> path = new();
        private readonly IWeightedGraph<Point> graph;

        public Pathfinding(IWeightedGraph<Point> graph)
        {
            this.graph = graph;
        }

        private static int Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private static List<Vector2> SimplifyPath(List<Vector2> path)
        {
            var simplified = new List<Vector2>();
            var directionOld = Vector2.Zero;

            for (int i = 1; i < path.Count; i++)
            {
                var directionNew = path[i - 1] - path[i];
                if (directionNew != directionOld)
                    simplified.Add(path[i]);
                directionOld = directionNew;
            }

            // TODO: This shouldn't be necessary!
            var last = path.Last();
            if (!simplified.Contains(last))
                simplified.Add(path.Last());

            return simplified;
        }

        public List<Vector2> FindPath(Vector2 start, Vector2 goal)
        {
            cameFrom.Clear();
            costSoFar.Clear();
            path.Clear();

            var startNode = new Point((int)start.X / Level.CellSize, (int)start.Y / Level.CellSize);
            var goalNode = new Point((int)goal.X / Level.CellSize, (int)goal.Y / Level.CellSize);

            var frontier = new PriorityQueue<Point, int>();
            frontier.Enqueue(startNode, 0);

            cameFrom[startNode] = startNode;
            costSoFar[startNode] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                // Reconstruct path and terminate.
                if (current == goalNode)
                {
                    while (current != startNode)
                    {
                        var waypoint = new Vector2(
                            current.X * Level.CellSize + Level.CellSize / 2f,
                            current.Y * Level.CellSize + Level.CellSize / 2f);
                        path.Add(waypoint);
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
                        var priority = newCost + Heuristic(next, goalNode);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            return SimplifyPath(path);
        }
    }
}
