using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Powerups
{
    public class TimeSlowdown : PowerupEffect
    {
        public TimeSlowdown(Hero hero, float duration) : base(hero, duration)
        {
        }

        public override void Off()
        {
        }

        public override void On()
        {
            foreach (var e in hero.GameplayScreen.AllEntities)
            {
                e.DX = MathF.Sign(e.DX) * .05f;
                e.DY = MathF.Sign(e.DY) * .05f;
            }
        }
    }

    public class HourGlass : Powerup
    {
        public HourGlass(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 48, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(animID);
        }

        protected override PowerupEffect CreateEffect()
        {
            return new TimeSlowdown(GameplayScreen.Hero, duration: 10f);
        }
    }
}
