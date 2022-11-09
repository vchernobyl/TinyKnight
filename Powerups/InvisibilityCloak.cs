using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Powerups
{
    public class Invisibility : PowerupEffect
    {
        public Invisibility(Hero hero) : base(hero, duration: 5f) { }

        public override void On()
        {
            var invisibilityColor = Color.White * .35f;
            hero.Sprite.Color = invisibilityColor;
            hero.Weapon.Sprite.Color = invisibilityColor;
            hero.Collisions &= ~Mask.Enemy;
        }

        public override void Off()
        {
            hero.Sprite.Color = Color.White;
            hero.Weapon.Sprite.Color = Color.White;
            hero.Collisions |= Mask.Enemy;
        }
    }

    public class InvisibilityCloak : Powerup
    {
        public InvisibilityCloak(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 32, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(animID);
        }

        protected override PowerupEffect CreateEffect()
        {
            return new Invisibility(GameplayScreen.Hero);
        }
    }
}
