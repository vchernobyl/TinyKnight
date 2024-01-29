using TinyKnight.Graphics;
using TinyKnight.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight.Powerups
{
    public class Berserk : PowerupEffect
    {
        private Weapon? weapon;
        private bool blinkToggle;
        private float blinkTime;
        private float time;

        public Berserk(Hero hero, float duration) : base(hero, duration)
        {
        }

        public override void Off()
        {
            hero.Sprite.Color = Color.White;
            hero.EquipWeapon(weapon);
            hero.Invincible = false;
        }

        public override void On()
        {
            weapon = hero.Weapon;
            hero.Sprite.Color = Color.Red;
            hero.EquipWeapon(null);
            hero.Invincible = true;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            time += gameTime.DeltaTime();

            var percentage = 1f - time / duration;
            if (percentage < .3f)
            {
                blinkTime += gameTime.DeltaTime();
                var blinkFor = MathHelper.Max(percentage, .2f);
                if (blinkTime >= blinkFor)
                {
                    blinkTime = 0f;
                    blinkToggle = !blinkToggle;
                }

                hero.Sprite.Color = blinkToggle ? Color.Red : Color.White;
            }
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

        protected override PowerupEffect CreateEffect()
        {
            return new Berserk(GameplayScreen.Hero, duration: 7f);
        }
    }
}
