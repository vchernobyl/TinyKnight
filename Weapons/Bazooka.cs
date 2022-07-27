using Microsoft.Xna.Framework;
using System.Threading;

namespace Gravity
{
    public class Explosion : Entity
    {
        private float time = 0;
        private readonly Curve sizeOverTime;

        public Explosion(GameplayScreen gameplayScreen) 
            : base(gameplayScreen, new Sprite(Textures.Circle))
        {
            Gravity = 0f;
            Radius = sprite.Size.X / 2;
            sprite.Color = Color.Black;
            sprite.Scale = Vector2.One * .1f;

            sizeOverTime = new Curve();
            sizeOverTime.Keys.Add(new CurveKey(0f, .45f));
            sizeOverTime.Keys.Add(new CurveKey(.1f, 1f));
        }

        public override void Update(GameTime gameTime)
        {
            time += gameTime.DeltaTimeF();

            sprite.Scale = Vector2.One * sizeOverTime.Evaluate(time);

            if (time >= .14f)
            {
                Flash(.06f, Color.White);
            }
            if (time >= .2f)
            {
                Thread.Sleep(millisecondsTimeout: 20);
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

        private readonly ParticleSystem trailParticles;
        private readonly ParticleEmitter trailEmitter;
        private readonly Game game;

        public Rocket(GameplayScreen gameplayScreen) 
            : base(gameplayScreen, new Sprite(Textures.Bullet))
        {
            game = gameplayScreen.ScreenManager.Game;
            trailParticles = new ParticleSystem(gameplayScreen.ScreenManager.Game,
                "Particles/RocketTrailSettings");
            game.Components.Add(trailParticles);

            trailEmitter = new ParticleEmitter(trailParticles, 60, Position);

            sprite.Color = Color.Red;
            Gravity = 0f;
        }

        public void Explode()
        {
            IsActive = false;
            gameplayScreen.AddEntity(new Explosion(gameplayScreen) { Position = Position });
            GravityGame.WorldCamera.Shake(.85f);
            SoundFX.Explosion.Play();
            game.Components.Remove(trailParticles);
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Damageable && other is IEnemy)
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
            trailEmitter.Update(gameTime, Position);
        }
    }

    public class Bazooka : Weapon
    {
        private readonly GameplayScreen gameplayScreen;

        public Bazooka(GameplayScreen gameplayScreen, Hero hero)
            : base(gameplayScreen, hero, fireRate: 1f, name: "Bazooka")
        {
            this.gameplayScreen = gameplayScreen;
        }

        public override void Shoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing * .75f, 0f);
            gameplayScreen.AddEntity(new Rocket(gameplayScreen) { Position = position, Velocity = velocity });
            SoundFX.BazookaShot.Play();
            GravityGame.WorldCamera.Shake(.6f);
        }
    }
}
