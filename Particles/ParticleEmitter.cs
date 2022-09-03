using Microsoft.Xna.Framework;

namespace Gravity.Particles
{
    /// <summary>
    /// Helper for objects that want to leave particles behind them
    /// as they move around the world. This emitter implementation
    /// solves two related problems:
    /// 
    /// If an object wants to create particles very slowly, less than
    /// once per frame, it can be a pain to keep track of which updates
    /// ought to create a new particle versus which should not.
    /// 
    /// If an object is moving quickly and is creating many particles
    /// per frame, it will look ugly if these particles are all bunched
    /// up together. Much better if they can be spread out along a line
    /// between where the object is now and where it was on the previous
    /// frame. This is particularly important for leaving trails behind
    /// fast moving objects such as rockets.
    /// 
    /// This emitter class keeps track of a moving object, remembering its
    /// previous position so it can calculate the velocity of the object.
    /// It works out the perfect locations for creating particles at any
    /// frequency you specify, regardless of whether this is faster or
    /// slower than the game update rate.
    /// </summary>
    public class ParticleEmitter
    {
        public Vector2 Position { get; private set; }

        private ParticleSystem particleSystem;
        private float timeBetweenParticles;
        private float timeLeftOver;

        public ParticleEmitter(ParticleSystem particleSystem,
            float particlesPerSecond, Vector2 initialPosition)
        {
            this.particleSystem = particleSystem;
            timeBetweenParticles = 1f / particlesPerSecond;
            Position = initialPosition;
        }

        public void Update(GameTime gameTime, Vector2 newPosition)
        {
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime > 0f)
            {
                // Work out how fast we are moving.
                var velocity = (newPosition - Position) / elapsedTime;

                // If we had any time left over that we didn't use during
                // the previous update, add that to the current elapsed time.
                var timeToSpend = timeLeftOver + elapsedTime;

                var currentTime = -timeLeftOver;
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    // Work out the optimal position for this particle. This
                    // will produce evenly spaced particles regardless of the
                    // object speed, particle creation frequency, or game update rate.
                    var mu = currentTime / elapsedTime;
                    var particlePosition = Vector2.Lerp(Position, newPosition, mu);
                    particleSystem.AddParticles(particlePosition, velocity);
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }

            Position = newPosition;
        }
    }
}
