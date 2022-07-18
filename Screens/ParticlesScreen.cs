using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class ParticlesScreen : GameScreen
    {
        private ParticleEmitter emitter;

        public override void LoadContent()
        {
            var particles = new ParticleSystem(ScreenManager.Game, "Particles/RocketTrailSettings");
            ScreenManager.Game.Components.Add(particles);
            emitter = new ParticleEmitter(particles, 60, new Vector2(400, 250));
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Input.WasKeyPressed(Keys.R))
                LoadContent();

            var position = Mouse.GetState().Position.ToVector2();
            emitter.Update(gameTime, position);
        }
    }
}
