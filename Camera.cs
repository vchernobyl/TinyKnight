using Microsoft.Xna.Framework;
using System;

namespace Gravity
{
    public class Camera
    {
        public Vector2 Position;
        public Matrix Transform => Matrix.CreateTranslation(ShakyPosition.X, ShakyPosition.Y, 0f);

        private Vector2 shakeOffset;
        private Vector2 ShakyPosition => Position + shakeOffset;

        private const float MaxOffset = 35f;
        private const float ShakeDecrease = 0.03f;

        private float trauma = 0f;

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
