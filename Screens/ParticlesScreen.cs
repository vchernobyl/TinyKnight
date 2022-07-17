using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class ParticlesScreen : GameScreen
    {
        private ParticleSystem trail;

        public override void LoadContent()
        {
            //trail = new RocketTrailParticles((GravityGame)ScreenManager.Game, 1);
            //ScreenManager.Game.Components.Add(trail);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //var where = Mouse.GetState().Position.ToVector2();
            //trail.AddParticles(where);
        }
    }
}
