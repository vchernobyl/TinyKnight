using TinyKnight.AI;
using TinyKnight.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight.Entities
{
    public class Demon : Enemy
    {
        public Demon(GameplayScreen gameplayScreen)
            : base(gameplayScreen, health: 100)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;

            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Demon"));
            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Midground;
            Sprite.Play(animID);
        }
    }
}
