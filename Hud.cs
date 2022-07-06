using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Hud
    {
        private readonly Game game;

        public Hud(Game game)
        {
            this.game = game;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(Fonts.Default, $"Enemies destroyed: {game.Hero.EnemiesKilled}", new Vector2(10f, 5f), Color.White);
        }
    }
}
