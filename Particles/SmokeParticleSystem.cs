using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class SmokePlumeParticleSystem : ParticleSystem
    {
        private static readonly Properties properties = new(
            TextureFilename: "Textures/smoke",
            MinInitialSpeed: 20,
            MaxInitialSpeed: 100,
            MinAcceleration: 0,
            MaxAcceleration: 0,
            MinLifetime: 5f,
            MaxLifetime: 7f,
            MinScale: .5f,
            MaxScale: 1f,
            MinNumParticles: 7,
            MaxNumParticles: 15,
            MinRotationSpeed: -MathHelper.PiOver4 / 2f,
            MaxRotationSpeed: MathHelper.PiOver4 / 2f,
            BlendState: BlendState.AlphaBlend);

        public SmokePlumeParticleSystem(Game game, int howManyEffects)
            : base(game, properties, howManyEffects)
        {
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
