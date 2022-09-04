using Microsoft.Xna.Framework;

namespace Gravity.Particles
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public float Lifetime { get; set; }
        public float TimeSinceStart { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public bool Active => TimeSinceStart < Lifetime;

        public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration,
            float lifetime, float scale, float rotationSpeed)
        {
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Lifetime = lifetime;
            Scale = scale;
            RotationSpeed = rotationSpeed;
            TimeSinceStart = 0f;
            Rotation = rotationSpeed != 0 ? Random.FloatRange(0, MathHelper.TwoPi) : 0f;
        }

        public void Update(float deltaTime)
        {
            Velocity += Acceleration * deltaTime;
            Position += Velocity * deltaTime;
            Rotation += RotationSpeed * deltaTime;
            TimeSinceStart += deltaTime;
        }
    }
}
