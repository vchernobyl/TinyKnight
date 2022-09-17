using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class SandboxScreen : GameScreen
    {
        #region Fields and Properties
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphics;
        private SpriteFont font;

        ParticleSystem explosion;
        ParticleSystem smoke;
        ParticleSystem smokePlume;

        ParticleEmitter emitter;
        ParticleSystem emitterSystem;

        private enum State
        {
            Explosions,
            SmokePlume,
            Emitter,
        }

        private const int NumStates = 3;
        private State currentState = State.Explosions;

        private const float TimeBetweenExplosions = 2f;
        private float timeTillExplosion = 0f;

        private const float TimeBetweenSmokePlumePuffs = .5f;
        private float timeTillPuff = 0f;
        #endregion

        public override void LoadContent()
        {
            spriteBatch = ScreenManager.SpriteBatch;
            graphics = ScreenManager.Game.GraphicsDevice;

            var content = ScreenManager.Game.Content;
            font = content.Load<SpriteFont>("Fonts/font");

            var game = ScreenManager.Game;

            explosion = new ParticleSystem(game, "Particles/ExplosionSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            game.Components.Add(explosion);

            smoke = new ParticleSystem(game, "Particles/ExplosionSmokeSettings") { DrawOrder = ParticleSystem.AlphaBlendDrawOrder };
            game.Components.Add(smoke);

            smokePlume = new ParticleSystem(game, "Particles/SmokePlumeSettings") { DrawOrder = ParticleSystem.AlphaBlendDrawOrder };
            game.Components.Add(smokePlume);

            emitterSystem = new ParticleSystem(game, "Particles/EmitterSettings") { DrawOrder = ParticleSystem.AlphaBlendDrawOrder };
            game.Components.Add(emitterSystem);

            emitter = new ParticleEmitter(emitterSystem, 60, new Vector2(400, 240));
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input.IsNewKeyPress(Keys.Space, null, out _))
                currentState = (State)((int)(currentState + 1) % NumStates);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (currentState)
            {
                case State.Explosions:
                    UpdateExplosions(dt);
                    break;
                case State.SmokePlume:
                    UpdateSmokePlume(dt);
                    break;
                case State.Emitter:
                    UpdateEmitter(gameTime);
                    break;
            }
        }

        private void UpdateEmitter(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var newPosition = new Vector2(mouseState.X, mouseState.Y);
            emitter.Update(gameTime, newPosition);
        }

        private void UpdateSmokePlume(float dt)
        {
            // Smoke.
            timeTillPuff -= dt;
            if (timeTillPuff < 0f)
            {
                var where = Vector2.Zero;
                where.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                where.Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
                smokePlume.AddParticles(where, Vector2.Zero);
                timeTillPuff = TimeBetweenSmokePlumePuffs;
            }
        }

        private void UpdateExplosions(float dt)
        {
            // Explosion.
            timeTillExplosion -= dt;
            if (timeTillExplosion < 0)
            {
                Vector2 where = Vector2.Zero;
                // Create the explosion at some random point on the screen.
                where.X = Random.IntRange(0, graphics.Viewport.Width);
                where.Y = Random.IntRange(0, graphics.Viewport.Height);

                // The overall explosion effect is actually comprised of two particle
                // systems: the fiery bit, and the smoke behind it. add particles to
                // both of those systems.
                explosion.AddParticles(where, Vector2.Zero);
                smoke.AddParticles(where, Vector2.Zero);

                // reset the timer.
                timeTillExplosion = TimeBetweenExplosions;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            graphics.Clear(Color.Black);

            spriteBatch.Begin();

            // Draw some instructions on the screen.
            string message = string.Format("Current effect: {0}!\n" +
                "Hit the A button or space bar, or tap the screen, to switch.\n\n" +
                "Free particles:\n" +
                "    ExplosionParticleSystem:      {1}\n" +
                "    ExplosionSmokeParticleSystem: {2}\n" +
                "    SmokePlumeParticleSystem:     {3}\n" +
                "    EmitterParticleSystem:        {4}",
                currentState, explosion.FreeParticleCount,
                smoke.FreeParticleCount, smokePlume.FreeParticleCount,
                emitterSystem.FreeParticleCount);
            spriteBatch.DrawString(font, message, new Vector2(50, 50), Color.White);

            spriteBatch.End();
        }
    }
}
