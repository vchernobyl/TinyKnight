using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Weapons
{
    // Handles efficient rendering automatically for its
    // users, in a similar way to SpriteBatch. It can render
    // lines, points, rectangles and circles to the screen.
    public class PrimitiveBatch : IDisposable
    {
        // This constant controls how large the vertex buffer is. Larger
        // buffers will require flushing less often, which can increase perfrormance.
        // However, having buffer that is unnecessarily large will waste memory.
        private const int DefaultBufferSize = 500;

        // A block of vertices that calling AddVertex will fill.
        // Flush will draw using this array, and will determine how
        // many primitives to draw from positionInBuffer.
        private VertexPositionColor[] vertices = new VertexPositionColor[DefaultBufferSize];

        // Keeps track of how many vertices have been added. This value
        // increases until we run out of space in the buffer, at which
        // time flushing is automatically done.
        private int positionInBuffer = 0;

        private BasicEffect basicEffect;
        private GraphicsDevice device;

        // This value is set by Begin, and is the type of primitives
        // that we are drawing.
        private PrimitiveType primitiveType;

        // How many vertices does each of these primitives take up?
        // Points are 1, lines are 2, triangles are 3.
        private int numVertsPerPrimitive;

        // This flag is flipped to true once Begin is called, and is used
        // to make sure users don't call End before Begin is called.
        private bool hasBegun = false;
        private bool isDispoed = false;

        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            device = graphicsDevice;
            basicEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = Matrix.CreateOrthographicOffCenter(
                left: 0, right: graphicsDevice.Viewport.Width,
                bottom: graphicsDevice.Viewport.Height, top: 0,
                zNearPlane: 0f, zFarPlane: 1f)
            };
        }

        public void Dispose()
        {
            if (!isDispoed)
            {
                basicEffect.Dispose();
                isDispoed = true;
            }
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitive will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType)
        {
            if (hasBegun)
            {
                throw new InvalidOperationException(
                    $"{nameof(End)} must be called before {nameof(Begin)} can be called again.");
            }

            // These thee types reuse vertices, so we can't flush properly without more
            // complex logic. Since that's a bit too complicated for this sample, we'll
            // simply disallow them for now.
            if (primitiveType == PrimitiveType.LineStrip ||
                primitiveType == PrimitiveType.TriangleList)
            {
                throw new NotSupportedException("The specified primitiveType is not supported.");
            }

            this.primitiveType = primitiveType;
            this.numVertsPerPrimitive = primitiveType switch
            {
                PrimitiveType.LineList => 2,
                PrimitiveType.TriangleList => 3,
                _ => throw new InvalidOperationException("Primitive is not valid."),
            };

            // Tell our basic effect to begin.
            basicEffect.CurrentTechnique.Passes[0].Apply();

            // Flip the error checking flag. It's now ok to call AddVertex, Flush, and End.
            hasBegun = true;
        }

        // End is called once all the primitives have been added using AddVertex.
        // It will call Flush to actually submit the draw call to the graphics card,
        // and then tell the basic effect to end.
        public void End()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException(
                    $"{nameof(Begin)} must be called before {nameof(End)} can be called.");
            }

            // Draw whatever is currently in the buffer.
            Flush();
            hasBegun = false;
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. For lines, twice and for triangle 3 times.
        // This function can only be called once Begin has been called.
        // If there is not enough room in the vertex buffer, Fluahs is called automatically.
        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException(
                    $"{nameof(Begin)} must be called before {nameof(AddVertex)} can be called.");
            }

            // TODO: Revisit this logic once we understand how the positionInBuffer is being set.
            bool newPrimitive = (positionInBuffer % numVertsPerPrimitive) == 0;
            if (newPrimitive && (positionInBuffer + numVertsPerPrimitive) >= vertices.Length)
                Flush();

            // Once we know there's enough room, set the vertex in the buffer, and increment position.
            vertices[positionInBuffer].Position = new Vector3(vertex, 0f);
            vertices[positionInBuffer].Color = color;
            positionInBuffer++;
        }

        // Flush is called to issue the draw call to the graphics card. Once the draw
        // call is made, the positionInBuffer is reset, so that AddVertex can start over
        // at the beginning. End will call this to draw the primitives that the user
        // requested, and AddVertex will call this if there is not enough room in the
        // buffer.
        private void Flush()
        {
            if (positionInBuffer == 0)
                return;

            int primitiveCount = positionInBuffer / numVertsPerPrimitive;

            // Submit the draw call to the graphics card.
            device.DrawUserPrimitives(primitiveType, vertices, vertexOffset: 0, primitiveCount);

            // Now that we've drawn, it's ok to reset the positionInBuffer back to zero,
            // and write over any vertices that may have been set previously.
            positionInBuffer = 0;
        }
    }
}
