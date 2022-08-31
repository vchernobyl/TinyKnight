using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Portal : Entity
    {
        public enum EnemyType
        {
            Walker, Flyer
        }

        public uint MaxEntities { get; set; } = 99;

        private readonly EnemyType enemyType;
        private readonly bool showDebugInfo = false;

        private readonly Timer spawnTimer;
        private readonly Timer destructionTimer;
        private readonly Timer firstTimeDelayTimer;

        private const float SpawnInterval = 10f;
        private const float DestructionDelay = 1f;

        private uint entitiesSpawned = 0;
        private float time = 0f;

        private readonly SoundEffectInstance explosionWindup;

        public Portal(Vector2 position, GameplayScreen gameplayScreen, EnemyType enemyType)
            : base(gameplayScreen, GetSprite(enemyType))
        {
            Position = position;
            Gravity = 0f;
            
            this.enemyType = enemyType;
            
            this.sprite.LayerDepth = 1f;
            
            this.spawnTimer = new Timer(SpawnInterval, Spawn, repeating: true, immediate: true);

            this.firstTimeDelayTimer = new Timer(1f, onEnd: spawnTimer.Start);
            this.firstTimeDelayTimer.Start();

            this.destructionTimer = new Timer(DestructionDelay, onEnd: Explode);
            this.explosionWindup = SoundFX.PortalExplosionWindup.CreateInstance();
        }

        private void Explode()
        {
            explosionWindup.Stop();
            SoundFX.PortalExplosion.Play();
            ScheduleToDestroy();
        }

        private void Spawn()
        {
            if (entitiesSpawned < MaxEntities && !destructionTimer.Started)
            {
                Damageable enemy = enemyType switch
                {
                    EnemyType.Flyer => new Bat(gameplayScreen),
                    EnemyType.Walker => new Bat(gameplayScreen),
                    _ => throw new ArgumentException($"Enemy type {enemyType} not supported!")
                };

                enemy.SetCoordinates(Position.X, Position.Y);
                enemy.OnDie += (_) => entitiesSpawned--;
                gameplayScreen.AddEntity(enemy);
                entitiesSpawned++;
            }
        }

        private static Sprite GetSprite(EnemyType enemyType)
        {
            var texture = enemyType switch
            {
                EnemyType.Flyer => Textures.PortalYellow,
                EnemyType.Walker => Textures.PortalOrange,
                _ => throw new ArgumentException($"Unsupported enemy type {enemyType}"),
            };
            return new Sprite(texture);
        }

        public override void Update(GameTime gameTime)
        {
            time += gameTime.DeltaTime();
            firstTimeDelayTimer.Update(gameTime);
            spawnTimer.Update(gameTime);
            destructionTimer.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            if (showDebugInfo)
            {
                var outline = new Rectangle(
                    (int)Position.X - Level.CellSize / 2,
                    (int)Position.Y - Level.CellSize / 2,
                    Level.CellSize,
                    Level.CellSize);
                batch.DrawRectangleOutline(outline, Color.Green, 2f);
            }
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Hero { IsLocked: false } hero)
            {
                Collision = false;
                hero.Lock(duration: DestructionDelay, this);
                hero.Gravity = 0f;
                destructionTimer.Start();
                explosionWindup.Play();
            }
        }
    }
}
