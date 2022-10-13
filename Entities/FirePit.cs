using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class FirePit : Entity
    {
        public FirePit(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            LevelCollisions = false;
            Gravity = 0f;

            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Fire"));

            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 4 * 8, 8), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Foreground;
            sprite.Play(animID);
        }

        public override void Update(GameTime gameTime)
        {
            // 
        }
    }
}
