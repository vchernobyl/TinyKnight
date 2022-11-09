using Gravity.Graphics;
using Gravity.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Powerups
{
    public class Berserk : Effect
    {
        private Weapon? weapon;

        public Berserk(Hero hero) : base(hero, duration: 10f)
        {
        }

        protected override void EffectOff()
        {
            Hero.Sprite.Color = Color.White;
            Hero.EquipWeapon(weapon);
            Hero.Invincible = false;
        }

        protected override void EffectOn()
        {
            weapon = Hero.Weapon;
            Hero.Sprite.Color = Color.Red;
            Hero.EquipWeapon(null);
            Hero.Invincible = true;
        }
    }

    public class BerserkersPotion : Powerup
    {
        public BerserkersPotion(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 40, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(animID);
        }

        protected override Effect CreateEffect()
        {
            return new Berserk(GameplayScreen.Hero);
        }
    }
}
