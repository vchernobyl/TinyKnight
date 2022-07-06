using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Portal : Damageable
    {
        public enum EnemyType
        {
            Walker, Flyer
        }

        public uint MaxEntities { get; init; } = 3;
        public double DelayBetweenSpawns { get; init; } = 2.0;

        private uint entitiesSpawned = 0;
        private double spawnTime = 0f;

        private readonly EnemyType enemyType;
        private readonly bool showDebugInfo = false;

        private readonly Timer recoveryTimer;

        private const double RecoveryTime = 5.0;

        private bool activated = true;

        public Portal(Vector2 position, Game game, EnemyType enemyType)
            : base(game, GetSprite(enemyType), health: 100)
        {
            Position = position;
            Gravity = 0f;
            this.enemyType = enemyType;
            this.sprite.LayerDepth = 1f;
            this.sprite.Scale = Vector2.One / 1.5f;
            this.recoveryTimer = new Timer(RecoveryTime, Reactivate);
        }

        private void Reactivate()
        {
            Heal(100);
            activated = true;
            sprite.Color = Color.White;
            recoveryTimer.Reset();
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
            recoveryTimer.Update(gameTime);

            spawnTime += gameTime.ElapsedGameTime.TotalSeconds;

            sprite.Rotation -= .025f;

            if (spawnTime >= DelayBetweenSpawns && entitiesSpawned < MaxEntities && activated)
            {
                spawnTime = 0.0;

                Damageable enemy = enemyType switch
                {
                    EnemyType.Flyer => new Flyer(game),
                    EnemyType.Walker => new Walker(game),
                    _ => throw new ArgumentException($"Enemy type {enemyType} not supported!")
                };

                enemy.SetCoordinates(Position.X, Position.Y);
                enemy.OnDie += (_) => entitiesSpawned--;
                game.AddEntity(enemy);
                entitiesSpawned++;

            }

            base.Update(gameTime);
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

        public override void Die()
        {
            activated = false;
            recoveryTimer.Start();
            sprite.Color = Color.Gray;
        }
    }
}
