using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Weapons
{
    public class Shield : Weapon
    {
        public Shield(Hero hero) 
            : base(hero, hero.GameplayScreen, fireRate: 1f, name: nameof(Shield), updateOrder: 200)
        {
            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8 * 8, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);

            Gravity = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void UpdatePosition()
        {
            Position = hero.Position + new Vector2(-2f * hero.Facing, 1f);
        }
    }
}
