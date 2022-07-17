using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Explosion : Entity
    {
        private double time = 0;
        private const float ExplosionRadius = 5f;

        public Explosion(GameplayScreen gameplayScreen) : base(gameplayScreen, new Sprite(Textures.Circle))
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

            foreach (var entity in gameplayScreen.Entities)
            {
                if (Overlaps(entity) && entity is Damageable enemy && enemy.IsAlive)
                    enemy.ReceiveDamage(100);
            }
        }
    }

    public class Rocket : Entity
    {
        public Vector2 Velocity { get; set; }

        private readonly RocketTrailParticles trailParticles;

        public Rocket(GameplayScreen gameplayScreen) : base(gameplayScreen, new Sprite(Textures.Bullet))
        {
            sprite.Color = Color.Red;
            Gravity = 0f;
            trailParticles = new RocketTrailParticles(
                (GravityGame)gameplayScreen.ScreenManager.Game, 1);
            gameplayScreen.ScreenManager.Game.Components.Add(trailParticles);
        }

        public void Explode()
        {
            IsActive = false;
            gameplayScreen.AddEntity(new Explosion(gameplayScreen) { Position = Position });
            GravityGame.WorldCamera.Shake(.765f);
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

            var where = Vector2.Zero;
            where.X = XX;
            where.Y = YY;
            trailParticles.Direction = -Vector2.Normalize(Velocity);
            trailParticles.AddParticles(where);
        }
    }

    public class Bazooka : Weapon
    {
        private readonly GameplayScreen gameplayScreen;
        private readonly Hero hero;

        public Bazooka(GameplayScreen gameplayScreen, Hero hero) : base(fireRate: 1f)
        {
            this.gameplayScreen = gameplayScreen;
            this.hero = hero;
        }

        public override void OnShoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing * .75f, 0f);
            gameplayScreen.AddEntity(new Rocket(gameplayScreen) { Position = position, Velocity = velocity });
            SoundFX.BazookaShot.Play();
        }
    }
}
