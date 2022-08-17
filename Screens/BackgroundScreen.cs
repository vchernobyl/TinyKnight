using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    /// <summary>
    /// The background screen sits behind the other menu screens.
    /// It draws a background image taht remains fixed in place
    /// regardless of whatever transitions the screens on top of
    /// it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);
        }

        /// <summary>
        /// Load graphics content for this screen. The background
        /// texture is quite big, so we use our own local ContentManager
        /// to load it. This allows us to unload before going from the
        /// menus into the game itself, wheras if we used the shared
        /// ContentManager provided by the Game class, the content would
        /// remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            content ??= new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");
            backgroundTexture = content.Load<Texture2D>("Textures/background");
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this
        /// should not transition off even if it has been covered by
        /// another screen: it is supposed to be covered, after all!
        /// This overload forces the coveredByOtherScreen parameter
        /// to false in order to stop the base Update method wanting
        /// to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            var color = new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, fullscreen, color);
            spriteBatch.End();
        }
    }
}
