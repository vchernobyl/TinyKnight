using Gravity.Entities;
using Gravity.Graphics;
using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Weapons
{
    public class CannonBall : Entity
    {
        private readonly Vector2 velocity;

        public CannonBall(GameplayScreen gameplayScreen, Vector2 velocity)
            : base(gameplayScreen)
        {
            this.velocity = velocity;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(8, 16, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Flip = velocity.X > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            // Don't destroy projectile if it touches ground or ceiling.
            if (normal.Y == 0f)
                Explode();
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy)
                Explode();
        }

        private void Explode()
        {
            GameplayScreen.AddEntity(new Explosion(GameplayScreen) { Position = Position });
            Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            DX = velocity.X;
            DY = velocity.Y;
        }
    }

    public class Cannon : Weapon
    {
        private readonly SoundEffect sound;
        private readonly ParticleSystem particles;

        public Cannon(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 2f, name: "Cannon", updateOrder: 100)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 16, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;

            sound = content.Load<SoundEffect>("SoundFX/Cannon_Shot");
            particles = new ParticleSystem(game, "Particles/Cannon_Shot");
            game.Components.Add(particles);

            Gravity = 0f;

            UpdatePosition();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void Shoot()
        {
            const float ballSpeed = 1f;
            var velocity = new Vector2(hero.Facing * ballSpeed, 0f);
            var position = hero.Position + new Vector2(hero.Facing * 6f, 1f);
            var ball = new CannonBall(GameplayScreen, velocity) { Position = position };

            GameplayScreen.AddEntity(ball);
            sound.Play();
            particles.AddParticles(position, Vector2.UnitX * hero.Facing * 100f);
            GravityGame.WorldCamera.Shake(.4f);

            hero.DX += -hero.Facing * .15f;
        }

        protected override void UpdatePosition()
        {
            Position = hero.Position + new Vector2(2f * hero.Facing, 1f);
            Sprite.Flip = hero.Facing > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        }
    }
}
