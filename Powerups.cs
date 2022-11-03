using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public abstract class Powerup : Entity
    {
        public Powerup(GameplayScreen gameplayScreen) : base(gameplayScreen) { }

        public abstract void ApplyEffectTo(Hero hero);

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Hero hero)
                ApplyEffectTo(hero);
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

        public override void ApplyEffectTo(Hero hero)
        {
            Destroy();
            hero.Sprite.Color = Color.White * .35f;
        }
    }
}
