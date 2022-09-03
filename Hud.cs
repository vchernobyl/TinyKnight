using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Hud
    {
        private readonly Sprite heart;
        private readonly Sprite emptyHeart;
        private readonly Hero hero;

        public Hud(GameplayScreen gameplayScreen, Hero hero)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var heartAtlas = content.Load<Texture2D>("Textures/Heart");
            this.heart = new Sprite(heartAtlas)
            {
                Source = new Rectangle(0, 0, 8, 8)
            };
            this.emptyHeart = new Sprite(heartAtlas)
            {
                Source = new Rectangle(16, 0, 8, 8)
            };
            this.hero = hero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < hero.Health; i++)
            {
                this.heart.Position = new Vector2(20f + i * 8f, 10f);
                this.heart.Draw(spriteBatch);
            }
            for (var i = Hero.MaxHealth; i > hero.Health && i >= 1; i--)
            {
                this.emptyHeart.Position = new Vector2(20f + (i - 1) * 8f, 10f);
                this.emptyHeart.Draw(spriteBatch);
            }
        }
    }
}
