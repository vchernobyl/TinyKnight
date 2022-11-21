using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Gravity
{
    public static class Bresenham
    {
        public static List<Point> GetLine(Point start, Point end)
        {
            return GetLine(start.X, start.Y, end.X, end.Y);
        }

        public static List<Point> GetLine(int x0, int y0, int x1, int y1)
        {
            var points = new List<Point>();
            var swapXY = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            int temp;
            if (swapXY)
            {
                // Swap x0 and y0.
                temp = x0;
                x0 = y0;
                y0 = temp;
                
                // Swap x1 and y1.
                temp = x1;
                x1 = y1;
                y1 = temp;
            }

            if (x0 > x1)
            {
                // Swap x0 and x1.
                temp = x0;
                x0 = x1;
                x1 = temp;

                // Swap y0 and y1.
                temp = y0;
                y0 = y1;
                y1 = temp;
            }

            var deltaX = x1 - x0;
            var deltaY = Math.Abs(y1 - y0);
            var error = deltaX / 2;
            var y = y0;
            var yStep = y0 < y1 ? 1 : -1;

            if (swapXY)
            {
                // Y / X
                for (var x = x0; x <= x1; x++)
                {
                    points.Add(new Point(x: y, y: x));
                    error -= deltaY;
                    if (error < 0)
                    {
                        y += yStep;
                        error += deltaX;
                    }
                }
            }
            else
            {
                // X / Y
                for (var x = x0; x <= x1; x++)
                {
                    points.Add(new Point(x, y));
                    error -= deltaY;
                    if (error < 0)
                    {
                        y += yStep;
                        error += deltaX;
                    }
                }
            }

            return points;
        }
    }
}
