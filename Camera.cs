using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Camera
    {
        public Vector2 Position;
        public float Scale = 1f;

        public Matrix Transform => Matrix.CreateTranslation(-Position.X + shakeOffset.X, -Position.Y + shakeOffset.Y, 0f) *
                                   Matrix.CreateScale(new Vector3(Scale, Scale, 1f)) *
                                   Matrix.CreateTranslation(bounds.Center.X, bounds.Center.Y, 0f);

        private Vector2 shakeOffset;

        private const float MaxOffset = 35f;
        private const float ShakeDecrease = 0.03f;

        private readonly Rectangle bounds;

        private float trauma;

        public Camera(Viewport viewport)
        {
            bounds = viewport.Bounds;
        }

        public void Update(GameTime _)
        {
            trauma = Math.Max(0f, trauma - ShakeDecrease);

            if (trauma > 0f)
            {
                var shake = trauma * trauma * trauma;
                shakeOffset.X = MaxOffset * shake * Random.FloatRange(-1f, 1f);
                shakeOffset.Y = MaxOffset * shake * Random.FloatRange(-1f, 1f);
            }
        }

        public void Shake(float trauma)
        {
            if (trauma > this.trauma)
                this.trauma = trauma;
        }
    }
}
