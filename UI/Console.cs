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
        private readonly List<string> output;
        private readonly List<string> history;
        private readonly CommandRegistry registry;

        private Rectangle rectangle;
        private float targetY;
        private float currentY;
        private int historyIndex;

        private const float OpenAmount = .45f;
        private const int CursorPaddingLeft = 5;
        private const int CursorPaddingBottom = 5;

        public bool IsOpen
        {
            get { return currentY > 0f; }
        }

        public IReadOnlyList<string> History
        {
            get { return history; }
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
            cursor = new Cursor(rectangle.Left + CursorPaddingLeft,
                rectangle.Bottom - CursorPaddingBottom,
                cursorWidth, cursorHeight,
                Color.White, blinkRate: .5f);

            textInput = new StringBuilder();
            output = new List<string>();
            history = new List<string>();
            registry = new CommandRegistry(game);

            game.Window.TextInput += HandleTextInput;
            game.Window.KeyDown += HandleKeyDown;
        }

        private void HandleKeyDown(object? sender, InputKeyEventArgs e)
        {
            if (e.Key == Keys.OemTilde)
                targetY = IsOpen ? 0f : OpenAmount;

            // History interaction.
            if (history.Count > 0 && IsOpen)
            {
                if (e.Key == Keys.Up)
                {
                    if (historyIndex > 0)
                        historyIndex--;

                    textInput.Clear();
                    textInput.Append(history[historyIndex]);
                }
                if (e.Key == Keys.Down)
                {
                    if (historyIndex < history.Count - 1)
                        historyIndex++;

                    textInput.Clear();
                    textInput.Append(history[historyIndex]);
                }
            }
        }

        private void HandleTextInput(object? sender, TextInputEventArgs e)
        {
            if (e.Key == Keys.OemTilde)
                return;

            if (currentY == 0f)
                return;

            // Prevent cursor blinking when typing.
            cursor.PauseBlink();

            switch (e.Key)
            {
                case Keys.Back:
                    if (textInput.Length > 0)
                        textInput.Remove(textInput.Length - 1, 1);
                    break;
                case Keys.Enter:
                    var input = textInput.ToString();
                    history.Add(input);
                    historyIndex = history.Count;
                    var args = input.Trim().Split(" ");
                    var commandName = args[0];
                    if (commandName == "")
                        output.Add("");
                    else
                    {
                        var command = registry.Find(commandName);
                        if (command == null)
                            output.Add($"Command `{commandName}` not found!");
                        else
                        {
                            var output = command.Procedure.Invoke(args[1..^0]).Split("\n");
                            foreach (var line in output)
                                this.output.Add(line);
                        }
                    }

                    textInput.Clear();

                    break;
            }

            if (font.Characters.Contains(e.Character))
                textInput.Append(e.Character);
        }

        public void Clear()
        {
            output.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            currentY = Numerics.Approach(currentY, targetY, gameTime.DeltaTime() * 4f);
            rectangle.Y = (int)(-height + currentY * height);

            cursor.Top = rectangle.Bottom - cursorHeight - 5;
            cursor.Left = rectangle.Left + (int)font.MeasureString(textInput).X + CursorPaddingLeft;
            cursor.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawRectangle(rectangle, backgroundColor);
            spriteBatch.DrawString(font, textInput, new Vector2(rectangle.Left + CursorPaddingLeft, cursor.Top), Color.White);

            var y = cursor.Top - cursorHeight;
            for (var i = output.Count - 1; i >= 0; i--)
            {
                var position = new Vector2(rectangle.Left + CursorPaddingLeft, y - (output.Count - 1 - i) * cursorHeight);
                spriteBatch.DrawString(font, output[i], position, Color.White);
            }

            cursor.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
