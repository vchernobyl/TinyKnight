using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity.UI
{
    public class Console : DrawableGameComponent
    {
        private readonly int width;
        private readonly int height;

        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont font;
        private readonly Color backgroundColor;
        private readonly Cursor cursor;
        private readonly int cursorWidth;
        private readonly int cursorHeight;

        private Rectangle rectangle;
        private float targetY;
        private float currentY;

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
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasKeyPressed(Keys.OemTilde))
                targetY = IsOpen ? 0f : .45f;

            currentY = Numerics.Approach(currentY, targetY, gameTime.DeltaTime() * 4f);
            rectangle.Y = (int)(-height + currentY * height);

            var leftPadding = 5;
            var bottomPadding = 10;
            cursor.Top = rectangle.Bottom - 20 - bottomPadding;
            cursor.Left = rectangle.Left + leftPadding;

            cursor.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawRectangle(rectangle, backgroundColor);
            cursor.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
