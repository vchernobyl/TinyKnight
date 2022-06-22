using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Hud
    {
        private readonly Hero hero;

        public Hud(Hero hero)
        {
            this.hero = hero;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(Fonts.Default, $"Enemies destroyed: {hero.EnemiesKilled}", new Vector2(10f, 5f), Color.White);
            batch.DrawString(Fonts.Default, $"Coins: {hero.Coins}", new Vector2(10f, 30f), Color.White);
        }
    }
}
