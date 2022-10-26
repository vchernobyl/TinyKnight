using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D? pixel;
        private static Texture2D BlankPixel(SpriteBatch spriteBatch)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData(new[] { Color.White });
            }
            return pixel;
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(BlankPixel(spriteBatch), rectangle, color);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Point position, Point size, Color color)
        {
            spriteBatch.Draw(BlankPixel(spriteBatch), new Rectangle(position, size), color);
        }

        // TODO: Why is this taking Point instead of Vector2? We should probably allow of drawing in floating point precision.
        public static void DrawRectangleOutline(this SpriteBatch spriteBatch, Point position, Point size, Color color, float thickenss = 1f)
        {
            var topRight = new Vector2(position.X + size.X, position.Y);
            DrawLine(spriteBatch, position.ToVector2(), topRight, color, thickenss);

            var bottomRight = (position + size).ToVector2();
            DrawLine(spriteBatch, topRight, bottomRight, color, thickenss);

            var bottomLeft = new Vector2(position.X, position.Y + size.Y);
            DrawLine(spriteBatch, bottomRight, bottomLeft, color, thickenss);
            DrawLine(spriteBatch, bottomLeft, position.ToVector2(), color, thickenss);
        }

        public static void DrawRectangleOutline(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, float thickness = 1f)
        {
            DrawRectangleOutline(spriteBatch, rectangle.Location, rectangle.Size, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 from, Vector2 to, Color color, float thickness)
        {
            var distance = Vector2.Distance(from, to);
            var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            DrawLine(spriteBatch, from, distance, angle, color, thickness);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 position, float radius, Color color, float thickness = 1f)
        {
            const int points = 256;

            var rotation = MathHelper.TwoPi / points;

            var sin = MathF.Sin(rotation);
            var cos = MathF.Cos(rotation);

            var ax = radius;
            var ay = 0f;

            for (var i = 0; i < points; i++)
            {
                var bx = cos * ax - sin * ay;
                var by = sin * ax + cos * ay;

                var from = new Vector2(position.X + ax, position.Y + ay);
                var to = new Vector2(position.X + bx, position.Y + by);

                spriteBatch.DrawLine(from, to, color, thickness);

                ax = bx;
                ay = by;
            }
        }

        private static void DrawLine(this SpriteBatch spriteBatch, Vector2 from, float distance, float angle, Color color, float thickness)
        {
            var origin = new Vector2(0f, .5f);
            var scale = new Vector2(distance, thickness);
            spriteBatch.Draw(BlankPixel(spriteBatch), from, null, color, angle, origin, scale, SpriteEffects.None, DrawLayer.Topmost);
        }
    }
}
