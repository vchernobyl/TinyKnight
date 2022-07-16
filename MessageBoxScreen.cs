using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    internal class MessageBoxScreen : GameScreen
    {
        #region Fields
        private readonly string message;
        private readonly InputAction menuSelect;
        private readonly InputAction menuCancel;
        private Texture2D? gradientTexture;
        #endregion

        #region Events
        public event EventHandler<PlayerIndexEventArgs>? Accepted;
        public event EventHandler<PlayerIndexEventArgs>? Cancelled;
        #endregion

        public MessageBoxScreen(string message) : this(message, false)
        {
            this.message = message;
        }

        public MessageBoxScreen(string message, bool includeUsageText)
        {
            var usage = @"\nA button, Space, Enter = ok
                          \nB button, Esc = cancel";

            if (includeUsageText)
                this.message += usage;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(.2);
            TransitionOffTime = TimeSpan.FromSeconds(.2);

            menuSelect = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Space, Keys.Enter },
                newPressOnly: true);

            menuCancel = new InputAction(
                new Buttons[] { Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape, Keys.Back },
                newPressOnly: true);
        }

        public override void LoadContent()
        {
            var content = ScreenManager.Game.Content;
            gradientTexture = content.Load<Texture2D>("Textures/gradient");
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                Accepted?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                Cancelled?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = font.MeasureString(message);
            var textPosition = (viewportSize / textSize) / 2;

            // The background includes a border somehwat larget than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle(
                (int)textPosition.X - hPad,
                (int)textPosition.Y - vPad,
                (int)textSize.X + hPad * 2,
                (int)textSize.Y + vPad * 2);

            var color = Color.White * TransitionAlpha;

            spriteBatch.Begin();
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);
            spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.End();
        }
    }
}