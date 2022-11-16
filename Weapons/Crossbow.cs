using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Weapons
{
    public class Crossbow : Weapon
    {
        private const float Spread = 0.025f;
        private const float ProjectileSpeed = .95f;
        private const int Damage = 40;

        private readonly SoundEffect shotSound;

        public Crossbow(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 8f, name: nameof(Crossbow), updateOrder: 100)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;

            shotSound = content.Load<SoundEffect>("SoundFX/Pistol_Shot");

            Gravity = 0f;

            UpdatePosition();
        }

        protected override void Shoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing * ProjectileSpeed, Random.FloatRange(-Spread, Spread));
            GameplayScreen.AddEntity(new Arrow(GameplayScreen, velocity, Damage) { Position = position });
            shotSound.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void UpdatePosition()
        {
            Position = hero.Position + new Vector2(3f * hero.Facing, 1f);
            Sprite.Flip = hero.Facing > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        }
    }
}
