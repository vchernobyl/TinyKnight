using Gravity.Coroutines;
using Gravity.Graphics;
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
            var red = new Color(255, 0, 77);
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

                yield return null;
            }
            Destroy();
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
            {
                GameplayScreen.AddEntity(new Explosion(GameplayScreen) { Position = Position });
                Destroy();
            }
        }

        public override void Update(GameTime gameTime)
        {
            DX = velocity.X;
            DY = velocity.Y;
        }
    }

    public class Cannon : Weapon
    {
        private readonly SoundEffect shotSound;

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

            shotSound = content.Load<SoundEffect>("SoundFX/Cannon_Shot");

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
            var ball = new CannonBall(GameplayScreen, velocity)
            {
                Position = hero.Position + new Vector2(hero.Facing * 6f, 1f)
            };

            GameplayScreen.AddEntity(ball);
            shotSound.Play();

            hero.DX += -hero.Facing * .3f;
        }
    }
}
