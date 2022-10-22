using Gravity.Coroutines;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
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
            sprite.Scale = Vector2.Zero;
            sprite.Color = Color.Gray;

            Gravity = 0f;

            var coroutine = game.Services.GetService<CoroutineRunner>();
            coroutine.Run(Expand());

            GravityGame.WorldCamera.Shake(.55f);
        }

        public IEnumerator Expand()
        {
            var frames = 0;
            while (frames++ < 8)
            {
                sprite.Color = Color.Lerp(Color.Gray, Color.White, frames / 8);
                sprite.Scale.X += .175f;
                sprite.Scale.Y += .175f;
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

            (DX, DY) = velocity;
            Gravity = 0f;
            FrictionX = 1f;
            FrictionY = 1f;
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
            //DX = velocity.X;
            //DY = velocity.Y;
        }
    }

    public class Cannon : Weapon
    {
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
        }
    }
}
