using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Hud
    {
        public uint EnemiesKilled = 0;

        private readonly SpriteFont font;
        private readonly Vector2 position;

        public Hud(Game game)
        {
            font = game.Content.Load<SpriteFont>("Fonts/Default");
            position = new Vector2(game.GraphicsDevice.Viewport.Width / 2, 10);
        }

        public void Draw(SpriteBatch batch)
        {
            var size = font.MeasureString($"{EnemiesKilled}");
            var offset = new Vector2(size.X / 2f, 0f);
            batch.DrawString(font, $"{EnemiesKilled}", position - offset, Color.White);
        }
    }
}
