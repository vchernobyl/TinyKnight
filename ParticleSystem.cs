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

        private Game game;
        private Sprite sprite;
        private int howManyEffects;

        private Particle[] particles;
        private Queue<Particle> freeParticles;

        public int FreeParticleCount => freeParticles.Count;

        // TODO: Can we refactor this into ParticleProps or something similar instead?
        #region Constants to be set by the subclasses
        protected int minNumParticles;
        protected int maxNumParticles;

        protected string textureFilename;

        protected float minInitialSpeed;
        protected float maxInitialSpeed;

        protected float minAcceleration;
        protected float maxAcceleration;

        protected float minRotationSpeed;
        protected float maxRotationSpeed;

        protected float minLifetime;
        protected float maxLifetime;

        protected float minScale;
        protected float maxScale;

        protected BlendState blendState;
        #endregion

        protected ParticleSystem(Game game, int howManyEffects) : base(game)
        {
            this.game = game;
            this.howManyEffects = howManyEffects;
        }

        public override void Initialize()
        {
            InitializeConstants();

            particles = new Particle[howManyEffects * maxNumParticles];
            freeParticles = new Queue<Particle>(howManyEffects * maxNumParticles);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle();
                freeParticles.Enqueue(particles[i]);
            }

            base.Initialize();
        }

        protected abstract void InitializeConstants();

        protected override void LoadContent()
        {
            // TODO: Maybe load default debug (purple) texture instead of failing?
            // Jason Gregory - GEA
            if (string.IsNullOrEmpty(textureFilename))
            {
                string message = @$"{nameof(textureFilename)} wasn't set properly,
                    so the particle system doesn't know what texture to load. Make sure your particle system's
                    {nameof(InitializeConstants)} function properly sets it.";
                throw new InvalidOperationException(message);
            }

            sprite = new Sprite(game.Content.Load<Texture2D>(textureFilename));

            base.LoadContent();
        }

        public void AddParticles(Vector2 where)
        {
            var numParticles = Random.IntRange(minNumParticles, maxNumParticles);
            for (int i = 0; i < numParticles && FreeParticleCount > 0; i++)
            {
                var p = freeParticles.Dequeue();
                InitializeParticle(p, where);
            }
        }

        protected virtual void InitializeParticle(Particle p, Vector2 where)
        {
            var direction = PickRandomDirection();
            var velocity = Random.FloatRange(minInitialSpeed, maxInitialSpeed);
            var acceleration = Random.FloatRange(minAcceleration, maxAcceleration);
            var lifetime = Random.FloatRange(minLifetime, maxLifetime);
            var scale = Random.FloatRange(minScale, maxScale);
            var rotationSpeed = Random.FloatRange(minRotationSpeed, maxRotationSpeed);
            
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
            game.SpriteBatch.Begin(SpriteSortMode.Deferred, blendState);
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
