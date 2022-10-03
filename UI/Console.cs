using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text;

namespace Gravity.UI
{
    public class Console : DrawableGameComponent
    {
        private readonly int width;
        private readonly int height;

        private readonly int cursorWidth;
        private readonly int cursorHeight;

        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont font;

        private readonly Color backgroundColor;
        private readonly Cursor cursor;

        private readonly StringBuilder textInput;
        private readonly List<string> history;

        private Rectangle rectangle;
        private float targetY;
        private float currentY;

        private const float OpenAmount = .45f;

        public bool IsOpen
        {
            get { return currentY > 0f; }
        }

        public Console(Game game) : base(game)
        {
            width = game.GraphicsDevice.Viewport.Width;
            height = game.GraphicsDevice.Viewport.Height;

            spriteBatch = game.Services.GetService<SpriteBatch>();

            font = game.Content.Load<SpriteFont>("Fonts/Default");

            backgroundColor = new Color(.2f, .4f, .6f, .85f);

            rectangle = new Rectangle(0, -height, width, height);

            (cursorWidth, cursorHeight) = font.MeasureString("M").ToPoint();
            cursor = new Cursor(rectangle.Left, rectangle.Bottom,
                cursorWidth, cursorHeight,
                Color.White, blinkRate: .75f);

            textInput = new StringBuilder();

            history = new List<string>();

            game.Window.TextInput += HandleTextInput;
        }

        private void HandleTextInput(object? sender, TextInputEventArgs e)
        {
            if (e.Key == Keys.OemTilde)
                return;

            // Prevent cursor blinking when typing.
            cursor.ResetBlinkTime();

            switch (e.Key)
            {
                case Keys.Back:
                    textInput.Remove(textInput.Length - 1, 1);
                    cursor.Left = (int)font.MeasureString(textInput).X;
                    break;
                case Keys.Enter:
                    history.Add(textInput.ToString());
                    textInput.Clear();
                    cursor.Left = (int)font.MeasureString(textInput).X;
                    break;
            }

            if (font.Characters.Contains(e.Character))
            {
                textInput.Append(e.Character);
                cursor.Left = (int)font.MeasureString(textInput).X;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasKeyPressed(Keys.OemTilde))
                targetY = IsOpen ? 0f : OpenAmount;

            currentY = Numerics.Approach(currentY, targetY, gameTime.DeltaTime() * 4f);
            rectangle.Y = (int)(-height + currentY * height);

            cursor.Top = rectangle.Bottom - cursorHeight;
            cursor.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawRectangle(rectangle, backgroundColor);
            spriteBatch.DrawString(font, textInput, new Vector2(rectangle.Left, cursor.Top), Color.White);

            var y = cursor.Top - cursorHeight;
            for (var i = history.Count - 1; i >= 0; i--)
            {
                var position = new Vector2(0f, y - (history.Count - 1 - i) * cursorHeight);
                spriteBatch.DrawString(font, history[i], position, Color.White);
            }

            cursor.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
