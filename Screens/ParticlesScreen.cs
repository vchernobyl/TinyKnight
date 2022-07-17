using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class RocketTrailParticleSystem : ParticleSystem
    {
        private static readonly Properties properties = new(
            TextureFilename: "Textures/Pixel",
            MinInitialSpeed: 20,
            MaxInitialSpeed: 100,
            MinAcceleration: 0,
            MaxAcceleration: 0,
            MinLifetime: 5f,
            MaxLifetime: 7f,
            MinScale: 5f,
            MaxScale: 10f,
            MinNumParticles: 70,
            MaxNumParticles: 150,
            MinRotationSpeed: -MathHelper.PiOver4 / 2f,
            MaxRotationSpeed: MathHelper.PiOver4 / 2f,
            BlendState: BlendState.AlphaBlend);

        public RocketTrailParticleSystem(Game game, int howManyEffects)
            : base(game, properties, howManyEffects)
        {
        }
    }

    public class ParticlesScreen : GameScreen
    {
        private ParticleSystem trail;

        public override void LoadContent()
        {
            trail = new RocketTrailParticleSystem((Game)ScreenManager.Game, 1);
            ScreenManager.Game.Components.Add(trail);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            var where = Vector2.Zero;
            where.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
            where.Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
            trail.AddParticles(where);
        }
    }
}
