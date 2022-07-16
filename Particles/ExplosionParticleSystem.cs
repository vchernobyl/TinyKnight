using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class ExplosionParticleSystem : ParticleSystem
    {
        private static readonly Properties properties = new(
            TextureFilename: "Textures/explosion",
            MinInitialSpeed: 40,
            MaxInitialSpeed: 500,
            MinAcceleration: 0,
            MaxAcceleration: 0,
            MinLifetime: .5f,
            MaxLifetime: 1f,
            MinScale: .3f,
            MaxScale: 1f,
            MinNumParticles: 20,
            MaxNumParticles: 50,
            MinRotationSpeed: -MathHelper.PiOver4,
            MaxRotationSpeed: MathHelper.PiOver4,
            BlendState: BlendState.Additive);

        public ExplosionParticleSystem(Game game, int howManyEffects)
            : base(game, properties, howManyEffects)
        {
            DrawOrder = AdditiveDrawOrder;
        }

        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);
            p.Acceleration = -p.Velocity / p.Lifetime;
        }
    }
}
