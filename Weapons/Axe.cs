using Gravity.Entities;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Weapons
{
    public class AxeProjectile : Entity
    {
        private readonly Vector2 velocity;

        public AxeProjectile(GameplayScreen gameplayScreen, Vector2 velocity)
            : base(gameplayScreen)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;

            this.velocity = velocity;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy)
                enemy.Damage(50);
        }

        public override void Update(GameTime gameTime)
        {
            DX = velocity.X;
            DY = velocity.Y;

            const float rotationSpeed = .75f;
            Sprite.Rotation += MathF.Sign(DX) * rotationSpeed;
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (normal.X != 0)
                Destroy();
        }
    }

    public class Axe : Weapon
    {
        private readonly SoundEffect sound;

        public Axe(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 3f, nameof(Axe), updateOrder: 200)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8, 8, 8), 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;

            sound = content.Load<SoundEffect>("SoundFX/Axe_Throw");

            Gravity = 0f;
            EntityCollisions = true;

            UpdatePosition();
        }

        protected override void Shoot()
        {
            const float speed = 1f;
            var velocity = new Vector2(hero.Facing * speed, 0f);
            var projectile = new AxeProjectile(GameplayScreen, velocity) { Position = Position };
            GameplayScreen.AddEntity(projectile);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void UpdatePosition()
        {
            Position = hero.Position + new Vector2(5f * hero.Facing, 0f);
            Sprite.Flip = hero.Facing > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        }
    }
}
