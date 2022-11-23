using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity.Graphics
{
    public class DebugShapeRenderer
    {
        private class DebugShape
        {
            /// <summary>
            /// The array of vertices the shape can use.
            /// </summary>
            public VertexPositionColor[] Vertices;

            /// <summary>
            /// The number of lines to draw for this shape.
            /// </summary>
            public int LineCount;

            /// <summary>
            /// The length of time to keep this shape visible.
            /// </summary>
            public float Lifetime;
        }

        // We use a cache system to reuse our DebugShape instances to avoid creating garbage.
        private static readonly List<DebugShape> cachedShapes = new List<DebugShape>();
        private static readonly List<DebugShape> activeShapes = new List<DebugShape>();

        // Allocate an array to hold our vertices; this will grow as needed by our renderer.
        private static VertexPositionColor[] verts = new VertexPositionColor[64];

        private static GraphicsDevice graphics;
        private static BasicEffect effect;

        [Conditional("DEBUG")]
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            graphics = graphicsDevice;
            effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = false,
                DiffuseColor = Vector3.One,
                World = Matrix.Identity,
                Projection = Matrix.CreateOrthographicOffCenter(
                    graphics.Viewport.Bounds, zNearPlane: 0f, zFarPlane: 1f),
            };
        }

        [Conditional("DEBUG")]
        public static void AddLine(Vector3 a, Vector3 b, Color color, float lifetime = 0f)
        {
            var shape = GetShapeForLines(1, lifetime);

            shape.Vertices[0] = new VertexPositionColor(a, color);
            shape.Vertices[1] = new VertexPositionColor(b, color);
        }

        [Conditional("DEBUG")]
        public static void AddLine(Vector2 a, Vector2 b, Color color, float lifetime = 0f)
        {
            var shape = GetShapeForLines(1, lifetime);

            shape.Vertices[0] = new VertexPositionColor(new Vector3(a, 0f), color);
            shape.Vertices[1] = new VertexPositionColor(new Vector3(b, 0f), color);
        }

        [Conditional("DEBUG")]
        public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color, float lifetime = 0f)
        {
            var shape = GetShapeForLines(3, lifetime);

            shape.Vertices[0] = new VertexPositionColor(a, color);
            shape.Vertices[1] = new VertexPositionColor(b, color);

            shape.Vertices[2] = new VertexPositionColor(b, color);
            shape.Vertices[3] = new VertexPositionColor(c, color);

            shape.Vertices[4] = new VertexPositionColor(c, color);
            shape.Vertices[5] = new VertexPositionColor(a, color);
        }

        [Conditional("DEBUG")]
        public static void AddRectangle(Rectangle rect, Color color, float lifetime = 0f)
        {
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public static void AddCircle(Vector2 center, float radius, float lifetime = 0f)
        {
            // TODO: Use Bresenham's algorithm to draw pixel perfect circle.
            throw new NotImplementedException();
        }

        [Conditional("DEBUG")]
        public static void Draw(GameTime gameTime, Matrix view)
        {
            // Update our effect with the matrices.
            effect.View = view;

            int vertexCount = 0;
            foreach (var shape in activeShapes)
                vertexCount += shape.LineCount * 2;

            if (vertexCount > 0)
            {
                // Make sure the vertex buffer is large enough.
                if (verts.Length < vertexCount)
                {
                    // If we have to resize, we make our array twice as large as necessary so
                    // we hopefully won't have to resize it for a while.
                    verts = new VertexPositionColor[vertexCount * 2];
                }

                // Now go through the shapes again to move the vertices to the buffer and
                // add up the number of lines to draw.
                var lineCount = 0;
                var vertIndex = 0;
                foreach (var shape in activeShapes)
                {
                    lineCount += shape.LineCount;
                    var shapeVerts = shape.LineCount * 2;
                    for (var i = 0; i < shapeVerts; i++)
                        verts[vertIndex++] = shape.Vertices[i];
                }

                // Start our effect to begin rendering.
                effect.CurrentTechnique.Passes[0].Apply();

                // We draw in a loop because the Reach profile only supports 65,535 primitives. While it's
                // not incredibly likely, if a game tries to render more than 65,535 lines we don't want to
                // crash. We handle this by doing a loop and drawing as many lines as we can at a time, capped
                // at our limit. We then move ahead in our vertex array and draw the next set of lines.
                var vertexOffset = 0;
                while (lineCount > 0)
                {
                    var linesToDraw = Math.Min(lineCount, 65_535);
                    graphics.DrawUserPrimitives(PrimitiveType.LineList, verts, vertexOffset, linesToDraw);
                    vertexOffset += linesToDraw * 2;
                    lineCount -= linesToDraw;
                }

                // Go through our active shapes and retire any shapes that have expired to the cache list.
                var resort = false;
                for (var i = activeShapes.Count - 1; i >= 0; i--)
                {
                    var s = activeShapes[i];
                    s.Lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (s.Lifetime <= 0f)
                    {
                        cachedShapes.Add(s);
                        activeShapes.RemoveAt(i);
                        resort = true;
                    }
                }

                // If we move any shapes around, we need to resort the cached list
                // to ensure that the smallest shapes are the first in the list.
                if (resort)
                    cachedShapes.Sort((s1, s2) => s1.Vertices.Length.CompareTo(s2.Vertices.Length));
            }
        }

        private static DebugShape GetShapeForLines(int lineCount, float lifetime)
        {
            DebugShape? shape = null;

            // We go through our cached list trying to find a shape that contains
            // a large enough array to hold our desired line count. If we find such
            // a shape, we move it from our cahced list to our active list and break
            // out of the loop.
            var vertCount = lineCount * 2;
            for (var i = 0; i < cachedShapes.Count; i++)
            {
                if (cachedShapes[i].Vertices.Length >= vertCount)
                {
                    shape = cachedShapes[i];
                    cachedShapes.RemoveAt(i);
                    activeShapes.Add(shape);
                    break;
                }
            }

            // If we didn't find a shape in our cache, we create a new shape and add it
            // to the active list.
            if (shape == null)
            {
                shape = new DebugShape { Vertices = new VertexPositionColor[vertCount] };
                activeShapes.Add(shape);
            }

            shape.LineCount = lineCount;
            shape.Lifetime = lifetime;

            return shape;
        }
    }

}
