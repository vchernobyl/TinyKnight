using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gravity.Particles
{
    public class ParticleSystem : DrawableGameComponent
    {
        // These two values control the order that particle systems
        // are drawn in. Typically, particles that use additive blending
        // should be drawn on top of particles that use regular alpha
        // blending. ParticleSystems should therefore set their DrawOrder
        // to the appropriate value in InitializeConstants, though it is
        // possible to use other values for more advanced effects.
        public const int AlphaBlendDrawOrder = 100;
        public const int AdditiveDrawOrder = 200;

        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Vector2 origin;

        // The list of particles used by this system. These are reused, so that
        // calling AddParticles will only cause allocations if we're trying
        // to create more particles thatn we have available.
        private readonly List<Particle> particles;

        // The queue of free particles keeps track of particles that are not
        // currently being used by an effect. When a new effect is requested,
        // particles are taken from this queue. When particles are finished
        // they are put onto this queue.
        private readonly Queue<Particle> freeParticles;

        private readonly string settingsAssetName;
        private ParticleSystemSettings settings;
        private BlendState blendState;

        public int FreeParticleCount => freeParticles.Count;

        public ParticleSystem(Game game, string settingsAssetName,
            int initialParticleCount = 10) : base(game)
        {
            this.settingsAssetName = settingsAssetName;

            // We create the particle list and queue with our initial count
            // and create that many particles. If we picked a reasonable value,
            // our system will not allocate any more objects after this point,
            // however the AddParticles method will allocate more particles
            // as needed.
            particles = new List<Particle>(initialParticleCount);
            freeParticles = new Queue<Particle>(initialParticleCount);
            for (int i = 0; i < initialParticleCount; i++)
            {
                particles.Add(new Particle());
                freeParticles.Enqueue(particles[i]);
            }
        }

        protected override void LoadContent()
        {
            settings = Game.Content.Load<ParticleSystemSettings>(settingsAssetName);
            texture = Game.Content.Load<Texture2D>(settings.TextureFilename);

            origin.X = texture.Width / 2f;
            origin.Y = texture.Height / 2f;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            blendState = new BlendState
            {
                AlphaSourceBlend = settings.SourceBlend,
                ColorSourceBlend = settings.SourceBlend,
                AlphaDestinationBlend = settings.DestinationBlend,
                ColorDestinationBlend = settings.DestinationBlend,
            };

            base.LoadContent();
        }

        /// <summary>
        /// AddParticles's job is to add an effect somewhere on the screen.
        /// If there aren't enough particles in the freeParticles queue, it
        /// will use as many as it can. This means that if there are not enough
        /// particles available, calling AddParticles will have no effect.
        /// </summary>
        public void AddParticles(Vector2 where, Vector2 velocity)
        {
            var numParticles = Random.IntRange(settings.MinNumParticles, settings.MaxNumParticles);

            // Create that many particles, if you can.
            for (int i = 0; i < numParticles; i++)
            {
                // If we're out of free particles, we allocate anothe ten particles
                // which should keep us going.
                if (freeParticles.Count == 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var newParticle = new Particle();
                        particles.Add(newParticle);
                        freeParticles.Enqueue(newParticle);
                    }
                }

                // Grab a particle from the freeParticles queue, and initialize it.
                var p = freeParticles.Dequeue();
                InitializeParticle(p, where, velocity);
            }
        }

        private void InitializeParticle(Particle p, Vector2 where, Vector2 velocity)
        {
            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            velocity *= settings.EmitterVelocitySensitivity;
            var direction = PickRandomDirection();
            var speed = Random.FloatRange(settings.MinInitialSpeed, settings.MaxInitialSpeed);
            velocity += direction * speed;

            var lifetime = Random.FloatRange(settings.MinLifetime, settings.MaxLifetime);
            var scale = Random.FloatRange(settings.MinSize, settings.MaxSize);
            var rotationSpeed = Random.FloatRange(settings.MinRotationSpeed, settings.MaxRotationSpeed);

            // Our settings angles are in degrees, so we must convert to radians.
            rotationSpeed = MathHelper.ToRadians(rotationSpeed);

            var acceleration = Vector2.Zero;
            switch (settings.AccelerationMode)
            {
                case AccelerationMode.Scalar:
                    var accelerationScale = Random.FloatRange(
                        settings.MinAccelerationScale,
                        settings.MaxAccelerationScale);
                    acceleration = direction * accelerationScale;
                    break;
                case AccelerationMode.EndVelocity:
                    acceleration = velocity * (settings.EndVelocity - 1f) / lifetime;
                    break;
                case AccelerationMode.Vector:
                    acceleration = new Vector2(
                        Random.FloatRange(settings.MinAccelerationVector.X, settings.MaxAccelerationVector.X),
                        Random.FloatRange(settings.MinAccelerationVector.Y, settings.MaxAccelerationVector.Y));
                    break;
                default:
                    break;
            }

            p.Initialize(where, velocity, acceleration, lifetime, scale, rotationSpeed);
        }

        protected virtual Vector2 PickRandomDirection()
        {
            var angle = Random.FloatRange(settings.MinDirectionAngle, settings.MaxDirectionAngle);
            angle = MathHelper.ToRadians(angle);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var p in particles)
            {
                if (p.Active)
                {
                    p.Acceleration += settings.Gravity * deltaTime;
                    p.Update(deltaTime);

                    // If that update finishes them, put them onto the free
                    // particles queue.
                    if (!p.Active)
                        freeParticles.Enqueue(p);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState,
                SamplerState.PointClamp,
                transformMatrix: GravityGame.WorldCamera.Transform);

            foreach (var p in particles)
            {
                // Skip inactive particles.
                if (!p.Active)
                    continue;

                // Normalized lifetime is a value from 0 to 1 and represents
                // how far a particle is through its life. 0 means it just
                // started, .5 is half way through, and 1 means it's just about
                // to be finished. This value will be used to calculate alpha
                // and scale, to avoid having particles suddenly appear or disappear.
                var normalizedLifetime = p.TimeSinceStart / p.Lifetime;

                // We want particles to fade in and out, so we'll calculate alpha
                // to be (normalizedLifetime) * (1 - normalizeLifetime). This way
                // when normalizeLifetime is 0 or 1, alpha is 0. The maximum value
                // is at normalizedLifetime .5, and is
                // (normalizedLifetime) * (1 - normalizedLifetime)
                // (.5) * (1 - .5)
                // .25
                // Since we want the maximum alpha to be 1, not .25, we'll scale
                // the entire equation by 4.
                var alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                var color = Color.Lerp(new Color(settings.StartColor / 255), new Color(settings.EndColor / 255),
                    normalizedLifetime) * alpha;

                // Make particles grow as they age. They'll start at 75% of their
                // size, and increase to 100% once they're finished.
                var scale = p.Scale * (.75f + .25f * normalizedLifetime);

                spriteBatch.Draw(texture, p.Position, sourceRectangle: null, color,
                    p.Rotation, origin, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
        }
    }
}
