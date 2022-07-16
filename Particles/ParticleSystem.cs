using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gravity
{
    public abstract class ParticleSystem : DrawableGameComponent
    {
        public const int AlphaBlendDrawOrder = 100;
        public const int AdditiveDrawOrder = 200;

        private readonly Game game;
        private readonly int howManyEffects;
        private readonly Properties properties;
        private Sprite sprite;

        private Particle[] particles;
        private Queue<Particle> freeParticles;

        public int FreeParticleCount => freeParticles.Count;

        public record Properties(
            string TextureFilename,

            int MinNumParticles,
            int MaxNumParticles,
            
            float MinInitialSpeed,
            float MaxInitialSpeed,
            
            float MinAcceleration,
            float MaxAcceleration,

            float MinRotationSpeed,
            float MaxRotationSpeed,

            float MinLifetime,
            float MaxLifetime,

            float MinScale,
            float MaxScale,
            
            BlendState BlendState);

        protected ParticleSystem(Game game, Properties properties, int howManyEffects) : base(game)
        {
            this.game = game;
            this.properties = properties;
            this.howManyEffects = howManyEffects;
        }

        // TODO: Move to the constructor.
        public override void Initialize()
        {
            particles = new Particle[howManyEffects * properties.MaxNumParticles];
            freeParticles = new Queue<Particle>(howManyEffects * properties.MaxNumParticles);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle();
                freeParticles.Enqueue(particles[i]);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Maybe load default debug (purple) texture instead of failing?
            // Jason Gregory - GEA
            if (string.IsNullOrEmpty(properties.TextureFilename))
            {
                string message = $"{nameof(properties.TextureFilename)} wasn't set properly";
                throw new InvalidOperationException(message);
            }

            sprite = new Sprite(game.Content.Load<Texture2D>(properties.TextureFilename));

            base.LoadContent();
        }

        public void AddParticles(Vector2 where)
        {
            var numParticles = Random.IntRange(properties.MinNumParticles, properties.MaxNumParticles);
            for (int i = 0; i < numParticles && FreeParticleCount > 0; i++)
            {
                var p = freeParticles.Dequeue();
                InitializeParticle(p, where);
            }
        }

        protected virtual void InitializeParticle(Particle p, Vector2 where)
        {
            var direction = PickRandomDirection();
            var velocity = Random.FloatRange(properties.MinInitialSpeed, properties.MaxInitialSpeed);
            var acceleration = Random.FloatRange(properties.MinAcceleration, properties.MaxAcceleration);
            var lifetime = Random.FloatRange(properties.MinLifetime, properties.MaxLifetime);
            var scale = Random.FloatRange(properties.MinScale, properties.MaxScale);
            var rotationSpeed = Random.FloatRange(properties.MinRotationSpeed, properties.MaxRotationSpeed);

            p.Initialize(where, velocity * direction, acceleration * direction,
                lifetime, scale, rotationSpeed);
        }

        protected virtual Vector2 PickRandomDirection()
        {
            var angle = Random.FloatRange(0, MathHelper.TwoPi);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var p in particles)
            {
                if (p.Active)
                {
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
            game.SpriteBatch.Begin(SpriteSortMode.Deferred, properties.BlendState);
            foreach (var p in particles)
            {
                // Skip inactive particles.
                if (!p.Active)
                    continue;

                // Is always between 0 and 1.
                var normalizedLifetime = p.TimeSinceStart / p.Lifetime;
                var alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                var color = Color.White * alpha;
                var scale = p.Scale * (.75f + .25f * normalizedLifetime);
                sprite.Position = p.Position;
                sprite.Color = color;
                sprite.Scale = scale;
                sprite.Draw(game.SpriteBatch);
            }
            game.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
