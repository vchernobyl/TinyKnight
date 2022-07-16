using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class ExplosionSmokeParticleSystem : ParticleSystem
    {
        private static readonly Properties properties = new(
            TextureFilename: "Textures/smoke",
            MinInitialSpeed: 20,
            MaxInitialSpeed: 200,
            MinAcceleration: -10,
            MaxAcceleration: -50,
            MinLifetime: 1f,
            MaxLifetime: 2.5f,
            MinScale: 1f,
            MaxScale: 2,
            MinNumParticles: 10,
            MaxNumParticles: 20,
            MinRotationSpeed: -MathHelper.PiOver4,
            MaxRotationSpeed: MathHelper.PiOver4,
            BlendState: BlendState.AlphaBlend);

        public ExplosionSmokeParticleSystem(Game game, int howManyEffects)
            : base(game, properties, howManyEffects)
        {
            DrawOrder = AlphaBlendDrawOrder;
        }
    }
}
