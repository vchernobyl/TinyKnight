using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gravity.Entities
{
    public class Guy : Entity
    {
        public Guy(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {

        }

        public override void Update(GameTime gameTime)
        {
            // The problem is that, since the Monogame/XNA input functionality is driven
            // by a global static class (which I think it should), we don't have an easy
            // way of preventing input handling based on some criteria (for example console
            // being open). This means we can't use Monogame input static classes directly.
            // We have a few options here:
            //
            // 1. Use the `Input` wrapper static class which would have additional logic
            //    of whether or not input is enabled or disabled.
            // 2. Use of HandleInput(InputState state) virtual function which would be
            //    called on every entity by the ScreenManager.

            if (Input.WasKeyPressed(Keys.Space))
            {
                DY = 10;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {

            }

            if (GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.A))
            {

            }
        }
    }
}
