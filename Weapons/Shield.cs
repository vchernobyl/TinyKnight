using Gravity.Entities;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Weapons
{
    public class ShieldProjectile : Entity
    {
        private const float MaxRange = 75f;
        private const uint MaxHits = 5;

        private Vector2 velocity;
        private Enemy? target;
        private int hitCount;

        public ShieldProjectile(GameplayScreen gameplayScreen, Vector2 velocity)
            : base(gameplayScreen)
        {
            this.velocity = velocity;

            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8 * 8, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);

            Category = Mask.Projectile;
            Collisions = Mask.Level | Mask.Enemy;

            Gravity = 0f;
            FrictionX = 1f;
            FrictionY = 1f;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy)
            {
                enemy.Damage(amount: 50);
                SetNextTarget(enemy);

                if (++hitCount >= MaxHits)
                {
                    Destroy();
                    return;
                }
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            SetNextTarget(null);
        }

        public override void Update(GameTime gameTime)
        {
            if (target != null)
                velocity = Vector2.Normalize(target.Position - Position);

            (DX, DY) = velocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DebugRenderer.AddCircle(Position, MaxRange, Color.Yellow);
        }

        private void SetNextTarget(Enemy? lastHitEnemy)
        {
            var nextTarget = ClosestTarget(lastHitEnemy);
            if (nextTarget != null)
            {
                target = nextTarget;
            }
            else
            {
                // If not targets are found within range, destroy the shield and maybe
                // play a sound cue and an animation or something.
                Destroy();
            }
        }

        // BUG: Currently the shield does not take into the account that
        // an enemy could be behind a wall. What we can do if check if there
        // was a collision with a wall while target is not null and destroy
        // the shield if that's the case.
        private Enemy? ClosestTarget(Enemy? lastHitEnemy)
        {
            float closestDistance = float.MaxValue;
            Enemy? closestEnemy = null;

            foreach (var entity in GameplayScreen.AllEntities)
            {
                if (entity is Enemy enemy && enemy != lastHitEnemy)
                {
                    var dist = Vector2.Distance(Position, entity.Position);
                    if (dist <= MaxRange && 
                        dist < closestDistance && 
                        !Level.CheckLineIsBlocked(Position, entity.Position))
                    {
                        closestDistance = dist;
                        closestEnemy = enemy;
                    }
                }
            }

            return closestEnemy;
        }
    }

    public class Shield : Weapon
    {
        public Shield(Hero hero)
            : base(hero, hero.GameplayScreen, fireRate: 1.5f, name: nameof(Shield), updateOrder: 200)
        {
            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8 * 8, 8, 8), duration: 0f);

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

        protected override void UpdatePosition()
        {
            Position = hero.Position + new Vector2(-2f * hero.Facing, -1f);
        }

        protected override void Shoot()
        {
            const float force = 1f;
            var velocity = new Vector2(hero.Facing * force, 0f);
            GameplayScreen.AddEntity(new ShieldProjectile(GameplayScreen, velocity)
            {
                Position = Position
            });
        }
    }
}
