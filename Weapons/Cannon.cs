using Gravity.Coroutines;
using Gravity.Entities;
using Gravity.Graphics;
using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Gravity.Weapons
{
    public class Explosion : Entity
    {
        public Explosion(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/ExplosionCircle"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 0, 32, 32), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
            sprite.LayerDepth = DrawLayer.Foreground;
            sprite.Scale = Vector2.One * .25f;

            Gravity = 0f;

            var coroutine = game.Services.GetService<CoroutineRunner>();
            coroutine.Run(Expand());

            var sound = content.Load<SoundEffect>("SoundFX/Explosion_2");
            sound.Play();

            GravityGame.WorldCamera.Shake(.55f);
        }

        public IEnumerator Expand()
        {
            var white = new Color(255, 241, 232);
            var yellow = new Color(255, 236, 39);

            var frames = 0;
            var totalDuration = 10f;
            while (frames++ < totalDuration)
            {
                if (frames <= 5)
                    sprite.Color = yellow;
                else if (frames <= 10)
                    sprite.Color = white;

                sprite.Scale.X += .13f;
                sprite.Scale.Y += .13f;

                const float spriteSize = 32f; // This needs to be changed if sprite size changes
                Radius = spriteSize / 2f * sprite.Scale.X;

                yield return null;
            }
            Destroy();
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy)
            {
                enemy.Damage(999);
                var xDirection = Vector2.Normalize(enemy.Position - Position).X;
                enemy.DX = xDirection * .5f;
                enemy.DY = -.75f;
                enemy.LevelCollisions = false;
            }
        }
    }

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

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
            sprite.LayerDepth = DrawLayer.Foreground;
            sprite.Flip = velocity.X > 0
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

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
            sprite.LayerDepth = DrawLayer.Foreground;

            sound = content.Load<SoundEffect>("SoundFX/Cannon_Shot");
            particles = new ParticleSystem(game, "Particles/Cannon_Shot");
            game.Components.Add(particles);

            LevelCollisions = false;
            Gravity = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position = hero.Position + new Vector2(2f * hero.Facing, 1f);
            sprite.Flip = hero.Facing > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
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
    }
}
