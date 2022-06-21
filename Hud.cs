using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Hud
    {
        private readonly Hero hero;
        private readonly SpriteFont font;

        public Hud(Game game)
        {
            hero = game.Hero;
            font = game.Content.Load<SpriteFont>("Fonts/Default");
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(font, $"Enemies destroyed: {hero.EnemiesKilled}", new Vector2(10f, 5f), Color.White);
            batch.DrawString(font, $"Coins: {hero.Coins}", new Vector2(10f, 30f), Color.White);
        }
    }
}
