using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class RocketTrailParticles : ParticleSystem
    {
        public Vector2 Direction { get; set; }

        private static readonly Properties properties = new(
            TextureFilename: "Textures/Pixel",
            MinInitialSpeed: 50,
            MaxInitialSpeed: 100,
            MinAcceleration: 0,
            MaxAcceleration: 0,
            MinLifetime: .05f,
            MaxLifetime: .15f,
            MinScale: 5f,
            MaxScale: 10f,
            MinNumParticles: 10,
            MaxNumParticles: 20,
            MinRotationSpeed: -MathHelper.PiOver4 / 2f,
            MaxRotationSpeed: MathHelper.PiOver4 / 2f,
            BlendState: BlendState.AlphaBlend);

        public RocketTrailParticles(GravityGame game, int howManyEffects)
            : base(game, properties, howManyEffects)
        {
        }

        protected override Vector2 PickRandomDirection()
        {
            return Direction;
        }
    }

    public class ParticlesScreen : GameScreen
    {
        private ParticleSystem trail;

        public override void LoadContent()
        {
            trail = new RocketTrailParticles((GravityGame)ScreenManager.Game, 1);
            ScreenManager.Game.Components.Add(trail);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            var where = Mouse.GetState().Position.ToVector2();
            trail.AddParticles(where);
        }
    }
}
