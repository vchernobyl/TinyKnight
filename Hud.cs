using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Hud
    {
        private readonly GameplayScreen gameplayScreen;

        public Hud(GameplayScreen gameplayScreen)
        {
            this.gameplayScreen = gameplayScreen;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(Fonts.Default,
                $"Enemies destroyed: {gameplayScreen.Hero.EnemiesKilled}",
                new Vector2(10f, 5f), Color.White);
        }
    }
}
