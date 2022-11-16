using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity.Graphics
{
    public static class DebugShapeRenderer
    {
        private interface IDebugShape
        {
            void Draw(SpriteBatch spriteBatch);
        }

        private class Line : IDebugShape
        {
            public Vector2 Start;
            public Vector2 End;
            public Color Color;
            public float Thickness;

            public void Draw(SpriteBatch spriteBatch)
            {
                var distance = Vector2.Distance(Start, End);
                var angle = (float)Math.Atan2(End.Y - Start.Y, End.X - Start.X);
                var origin = new Vector2(0f, .5f);
                var scale = new Vector2(distance, Thickness);
                spriteBatch.Draw(pixel, Start, null, Color, angle, origin, scale, SpriteEffects.None, 0f);
            }
        }

        private static Texture2D? pixel;
        private static SpriteBatch? batch;

        // List of all the shapes which are scheduled to be rendered in a current frame.
        private static readonly List<IDebugShape> shapes = new List<IDebugShape>();

        public static void Initialize(SpriteBatch spriteBatch)
        {
            batch = spriteBatch;
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        [Conditional("DEBUG")]
        public static void AddLine(Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            shapes.Add(new Line { Start = start, End = end, Color = color, Thickness = thickness });
        }

        [Conditional("DEBUG")]
        public static void Draw(GameTime gameTime)
        {
            if (batch == null)
                throw new Exception($"{nameof(DebugShapeRenderer)} was not initialized!");

            batch.Begin(transformMatrix: GravityGame.WorldCamera.Transform);
            foreach (var shape in shapes)
                shape.Draw(batch);
            batch.End();

            // Remove all scheduled shapes after all of them have been drawn.
            // This prevents them from being drawn again in the next frame.
            shapes.Clear();
        }
    }
}
