﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D pixel;
        private static Texture2D BlankPixel(SpriteBatch spriteBatch)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData(new[] { Color.White });
            }
            return pixel;
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Point position, Point size, Color color)
        {
            spriteBatch.Draw(BlankPixel(spriteBatch), new Rectangle(position, size), color);
        }

        public static void DrawRectangleOutline(this SpriteBatch spriteBatch, Point position, Point size, Color color, float thickenss)
        {
            var topRight = new Vector2(position.X + size.X, position.Y);
            DrawLine(spriteBatch, position.ToVector2(), topRight, color, thickenss);

            var bottomRight = (position + size).ToVector2();
            DrawLine(spriteBatch, topRight, bottomRight, color, thickenss);

            var bottomLeft = new Vector2(position.X, position.Y + size.Y);
            DrawLine(spriteBatch, bottomRight, bottomLeft, color, thickenss);
            DrawLine(spriteBatch, bottomLeft, position.ToVector2(), color, thickenss);
        }

        public static void DrawRectangleOutline(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, float thickness)
        {
            DrawRectangleOutline(spriteBatch, rectangle.Location, rectangle.Size, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 from, Vector2 to, Color color, float thickness)
        {
            var distance = Vector2.Distance(from, to);
            var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            DrawLine(spriteBatch, from, distance, angle, color, thickness);
        }

        private static void DrawLine(this SpriteBatch spriteBatch, Vector2 from, float distance, float angle, Color color, float thickness)
        {
            var origin = new Vector2(0f, .5f);
            var scale = new Vector2(distance, thickness);
            spriteBatch.Draw(BlankPixel(spriteBatch), from, null, color, angle, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
