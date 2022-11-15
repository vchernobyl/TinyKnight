using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Weapons
{
    public class Shard : Entity
    {
        private Vector2 velocity;
        private uint collisionCount;

        public Shard(GameplayScreen gameplayScreen, Vector2 velocity)
            : base(gameplayScreen)
        {
            this.velocity = velocity;

            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(8, 8 * 7, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);

            Collisions = Mask.Enemy | Mask.Level;
            Gravity = 0f;
            FrictionX = 1f;
            FrictionY = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            (DX, DY) = velocity;
            Sprite.Rotation = Numerics.VectorToRadians(velocity);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            const uint maxCollisions = 4;
            if (++collisionCount >= maxCollisions)
                Destroy();

            velocity = Vector2.Reflect(velocity, normal);
        }
    }

    public class ThrowingStarProjectile : Entity
    {
        public ThrowingStarProjectile(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8 * 7, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);

            Collisions = Mask.Enemy | Mask.Level;
        }

        public override void Update(GameTime gameTime)
        {
            var hero = GameplayScreen.Hero;
            Sprite.Rotation += .5f * hero.Facing;
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            Destroy();
            SpawnShards(5, normal);
        }

        private void SpawnShards(uint amount, Vector2 normal)
        {
            var spread = MathHelper.ToRadians(45f);

            for (var i = 0; i < amount; i++)
            {
                var radians = Numerics.VectorToRadians(normal) + Random.FloatRange(-spread, spread);
                var direction = Numerics.RadiansToVector(radians);
                var speed = .75f;
                var velocity = Vector2.Normalize(direction) * speed;
                GameplayScreen.AddEntity(new Shard(GameplayScreen, velocity)
                {
                    Position = Position,
                });
            }
        }
    }

    public class ThrowingStar : Weapon
    {
        public ThrowingStar(Hero hero)
            : base(hero, hero.GameplayScreen, fireRate: 1.5f, name: nameof(ThrowingStar), updateOrder: 200)
        {
            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8 * 7, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);

            Gravity = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void Shoot()
        {
            const float xForce = 0.8f;
            const float yForce = -0.3f;

            var projectile = new ThrowingStarProjectile(GameplayScreen)
            {
                Position = hero.Position,
                DX = hero.Facing * xForce,
                DY = yForce,
                FrictionX = .99f,
                FrictionY = .99f,
            };

            GameplayScreen.AddEntity(projectile);
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
