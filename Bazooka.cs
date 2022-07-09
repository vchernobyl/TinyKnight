using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Explosion : Entity
    {
        private double time = 0;
        private const float ExplosionRadius = 5f;

        public Explosion(Game game) : base(game, new Sprite(Textures.Circle))
        {
            sprite.Color = Color.Yellow;
            sprite.Scale *= 2f;
            Gravity = 0f;
            Radius = Level.CellSize / 2f * ExplosionRadius;
        }

        public override void Update(GameTime gameTime)
        {
            time += gameTime.DeltaTime();
            if (time >= .2f)
            {
                IsActive = false;
                return;
            }

            foreach (var entity in game.Entities)
            {
                if (Overlaps(entity) && entity is Damageable enemy && enemy.IsAlive)
                    enemy.ReceiveDamage(100);
            }
        }
    }

    public class Rocket : Entity
    {
        public Vector2 Velocity { get; set; }

        public Rocket(Game game) : base(game, new Sprite(Textures.Bullet))
        {
            sprite.Color = Color.Red;
            Gravity = 0f;
        }

        public void Explode()
        {
            IsActive = false;
            game.AddEntity(new Explosion(game) { Position = Position });
            game.WorldCamera.Shake(.765f);
            SoundFX.Explosion.Play();
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Damageable and IEnemy)
            {
                Explode();
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (normal == Vector2.UnitX || normal == -Vector2.UnitX)
                Explode();
        }

        public override void Update(GameTime gameTime)
        {
            DX = Velocity.X;
            DY = Velocity.Y;
        }
    }

    public class Bazooka : Weapon
    {
        private readonly Game game;
        private readonly Hero hero;

        public Bazooka(Game game, Hero hero) : base(fireRate: 1f)
        {
            this.game = game;
            this.hero = hero;
        }

        public override void OnShoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing * .75f, 0f);
            game.AddEntity(new Rocket(game) { Position = position, Velocity = velocity });
            SoundFX.BazookaShot.Play();
        }
    }
}
