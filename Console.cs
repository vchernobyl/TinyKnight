using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Console : DrawableGameComponent
    {
        private readonly int width;
        private readonly int height;

        private readonly SpriteBatch spriteBatch;
        private readonly Color backgroundColor;

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
            backgroundColor = new Color(.2f, .4f, .6f, .85f);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasKeyPressed(Keys.OemTilde))
                targetY = IsOpen ? 0f : .45f;

            currentY = Numerics.Approach(currentY, targetY, gameTime.DeltaTime() * 4f);
        }

        public override void Draw(GameTime gameTime)
        {
            var size = new Rectangle(0, (int)(-height + (currentY * height)), width, height);
            spriteBatch.Begin();
            spriteBatch.DrawRectangle(size, backgroundColor);
            spriteBatch.End();
        }
    }
}
