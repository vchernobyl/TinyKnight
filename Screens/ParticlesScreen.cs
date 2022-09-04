using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class ParticlesScreen : GameScreen
    {
        private ParticleEmitter emitter;
        private ParticleSystem particles;

        public override void LoadContent()
        {
            particles = new ParticleSystem(ScreenManager.Game, "Particles/RocketTrailSettings");
            ScreenManager.Game.Components.Add(particles);
            //emitter = new ParticleEmitter(particles, 60, new Vector2(400, 250));
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Input.WasKeyPressed(Keys.R))
                LoadContent();

            var position = Mouse.GetState().Position.ToVector2();
            var worldPoint = GravityGame.WorldCamera.ScreenToWorldSpace(position);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                particles.AddParticles(worldPoint, Vector2.Zero);
            }

            //emitter.Update(gameTime, worldPoint);
        }
    }
}
