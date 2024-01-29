using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight
{
    public class Hud
    {
        private readonly Hero hero;
        private readonly SpriteFont font;
        private readonly Vector2 center;

        private int score;

        public Hud(GameplayScreen gameplayScreen)
        {
            hero = gameplayScreen.Hero;
            hero.OnScoreUpdate += UpdateScore;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            font = content.Load<SpriteFont>("Fonts/Score");

            var screenWidth = game.GraphicsDevice.Viewport.Width;
            center = new Vector2(screenWidth / 2f, 20f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var text = $"{score}";
            var offset = font.MeasureString(text).X / 2f;
            var position = new Vector2(center.X - offset, center.Y);
            spriteBatch.DrawString(font, text, position, Color.White);
        }

        private void UpdateScore(int newScore)
        {
            score = newScore;
        }
    }
}
