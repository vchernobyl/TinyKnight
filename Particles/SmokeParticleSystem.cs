using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class SmokePlumeParticleSystem : ParticleSystem
    {
        public SmokePlumeParticleSystem(Game game, int howManyEffects)
            : base(game, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            textureFilename = "Textures/smoke";

            minInitialSpeed = 20;
            maxInitialSpeed = 100;

            // we don't want the particles to accelerate at all, aside from what we
            // do in our overriden InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // long lifetime, this can be changed to create thinner or thicker smoke.
            // tweak minNumParticles and maxNumParticles to complement the effect.
            minLifetime = 5.0f;
            maxLifetime = 7.0f;

            minScale = .5f;
            maxScale = 1.0f;

            // we need to reduce the number of particles on Windows Phone in order to keep
            // a good framerate
            minNumParticles = 7;
            maxNumParticles = 15;

            // rotate slowly, we want a fairly relaxed effect
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            blendState = BlendState.AlphaBlend;

            DrawOrder = AlphaBlendDrawOrder;
        }

        protected override Vector2 PickRandomDirection()
        {
            var radians = Random.FloatRange(MathHelper.ToRadians(80), MathHelper.ToRadians(100));
            var direction = Vector2.Zero;
            direction.X = MathF.Cos(radians);
            direction.Y = -MathF.Sin(radians);
            return direction;
        }

        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);
            p.Acceleration.X += Random.FloatRange(10, 50);
        }
    }
}
