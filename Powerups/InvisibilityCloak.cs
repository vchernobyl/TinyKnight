﻿using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Powerups
{
    public class Invisibility : Effect
    {
        public Invisibility(Hero hero) : base(hero, duration: 5f) { }

        protected override void EffectOn()
        {
            Hero.Sprite.Color = Color.White * .35f;
            Hero.Collisions &= ~Mask.Enemy;
        }

        protected override void EffectOff()
        {
            Hero.Sprite.Color = Color.White;
            Hero.Collisions |= Mask.Enemy;
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

        protected override Effect CreateEffect()
        {
            return new Invisibility(GameplayScreen.Hero);
        }
    }
}
