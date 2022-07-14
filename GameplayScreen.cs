using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace Gravity
{
    class GameplayScreen : GameScreen
    {
        #region Fields
        ContentManager content;
        SpriteFont gameFont;

        float pauseAlpha;
        InputAction pauseAction;
        #endregion

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                newPressOnly: true);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");

                gameFont = content.Load<SpriteFont>("Fonts/Default");

                // TODO: remove later, just for testing purposes how loading will look like
                // if the screen/content takes longer to load.
                Thread.Sleep(1000);

                // Once the load has finished, we use ResetElapsedTime to tell the game's
                // timining mechanism that we have just finished a very long frame, and
                // that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Unload()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
