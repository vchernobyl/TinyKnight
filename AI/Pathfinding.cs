using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gravity.AI
{
    public class Pathfinding
    {
        private readonly Dictionary<Point, Point> cameFrom;
        private readonly Dictionary<Point, int> costSoFar;
        private readonly List<Vector2> path;
        private readonly IWeightedGraph<Point> graph;

        public Pathfinding(IWeightedGraph<Point> graph)
        {
            this.cameFrom = new Dictionary<Point, Point>();
            this.costSoFar = new Dictionary<Point, int>();
            this.path = new List<Vector2>();
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

            var frontier = new PriorityQueue<Point>();
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
