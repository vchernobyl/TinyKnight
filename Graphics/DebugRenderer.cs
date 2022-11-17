using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity.Graphics
{
    public static class DebugRenderer
    {
        private abstract class DebugShape
        {
            public Color Color = Color.Green;
            public float Thickness = 1f;
            public float Lifetime = 0f; // 0 here means the shape will only be drawn for a single frame.

            public abstract void Draw(SpriteBatch spriteBatch);
        }

        private class DebugLine : DebugShape
        {
            public Vector2 Start;
            public Vector2 End;

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawLine(Start, End, Color, Thickness);
            }
        }

        private class DebugRectangle : DebugShape
        {
            public Vector2 Position;
            public Vector2 Size;

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawRectangleOutline(Position, Size, Color, Thickness);
            }
        }

        private class DebugCircle : DebugShape
        {
            public Vector2 Center;
            public float Radius;

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawCircle(Center, Radius, Color, Thickness);
            }
        }

        private static Texture2D? pixel;
        private static SpriteBatch? batch;

        // List of all the shapes which are scheduled to be rendered in a current frame.
        private static readonly List<DebugShape> shapes = new List<DebugShape>();

        [Conditional("DEBUG")]
        public static void Initialize(SpriteBatch spriteBatch)
        {
            batch = spriteBatch;
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        [Conditional("DEBUG")]
        public static void AddLine(Vector2 start, Vector2 end, Color color,
            float thickness = 1f, float lifetime = 0f)
        {
            var line = new DebugLine
            {
                Color = color,
                Thickness = thickness,
                Lifetime = lifetime,
                Start = start,
                End = end
            };
            shapes.Add(line);
        }

        [Conditional("DEBUG")]
        public static void AddRectangle(Vector2 topLeft, Vector2 size, Color color,
            float thickness = 1f, float lifetime = 0f)
        {
            var rectangle = new DebugRectangle
            {
                Color = color,
                Thickness = thickness,
                Lifetime = lifetime,
                Position = topLeft,
                Size = size
            };
            shapes.Add(rectangle);
        }

        [Conditional("DEBUG")]
        public static void AddCircle(Vector2 center, float radius, Color color,
            float thickness = 1f, float lifetime = 0f)
        {
            var circle = new DebugCircle
            {
                Color = color,
                Thickness = thickness,
                Lifetime = lifetime,
                Center = center,
                Radius = radius,
            };
            shapes.Add(circle);
        }

        [Conditional("DEBUG")]
        public static void Draw(GameTime gameTime)
        {
            if (batch == null)
                throw new Exception($"{nameof(DebugRenderer)} was not initialized!");

            batch.Begin(transformMatrix: GravityGame.WorldCamera.Transform);
            foreach (var shape in shapes)
            {
                shape.Draw(batch);
                shape.Lifetime -= gameTime.DeltaTime();
            }
            batch.End();

            // Remove all debug shape which have expired lifetime.
            for (int i = shapes.Count - 1; i >= 0; i--)
            {
                if (shapes[i].Lifetime <= 0f)
                    shapes.RemoveAt(i);
            }
        }
    }
}
