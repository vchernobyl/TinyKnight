﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class ExplosionParticleSystem : ParticleSystem
    {
        public ExplosionParticleSystem(Game game, int howManyEffects) 
            : base(game, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            textureFilename = "Textures/explosion";

            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 40;
            maxInitialSpeed = 500;

            // doesn't matter what these values are set to, acceleration is tweaked in
            // the override of InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosions should be relatively short lived
            minLifetime = .5f;
            maxLifetime = 1.0f;

            minScale = .3f;
            maxScale = 1.0f;

            minNumParticles = 20;
            maxNumParticles = 25;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            // additive blending is very good at creating fiery effects.
            blendState = BlendState.Additive;

            DrawOrder = AdditiveDrawOrder;
        }

        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);
            p.Acceleration = -p.Velocity / p.Lifetime;
        }
    }
}
