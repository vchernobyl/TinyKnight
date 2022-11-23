using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class SandboxScreen : GameScreen
    {
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphics;
        private ParticleSystem fire;

        public override void LoadContent()
        {
            spriteBatch = ScreenManager.SpriteBatch;
            graphics = ScreenManager.Game.GraphicsDevice;

            var game = ScreenManager.Game;
            fire = new ParticleSystem(game, "Particles/Fire");
            game.Components.Add(fire);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var position = GravityGame.WorldCamera.ScreenToWorldSpace(Mouse.GetState().Position.ToVector2());
                fire.AddParticles(position, Vector2.Zero);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            graphics.Clear(Color.Black);
        }
    }
}
