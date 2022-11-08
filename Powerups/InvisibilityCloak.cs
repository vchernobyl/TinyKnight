using Gravity.Coroutines;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Gravity.Powerups
{
    public class Invisibility : IEffect
    {
        public void ApplyEffect(Hero hero)
        {
            hero.Sprite.Color = Color.White * .35f;
            hero.Collisions &= ~Mask.Enemy;
        }

        public void DiscardEffect(Hero hero)
        {
            hero.Sprite.Color = Color.White;
            hero.Collisions |= Mask.Enemy;
        }
    }

    public class InvisibilityCloak : Powerup
    {
        private readonly Invisibility invisibility;
        private readonly CoroutineRunner coroutine;

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

            invisibility = new Invisibility();
            coroutine = game.Services.GetService<CoroutineRunner>();
        }

        public override void ApplyEffect(Hero hero)
        {
            invisibility.ApplyEffect(hero);
            coroutine.Run(StartTimer(hero));
            Destroy();
        }

        private IEnumerator StartTimer(Hero hero)
        {
            yield return 3f;
            invisibility.DiscardEffect(hero);
        }
    }
}
