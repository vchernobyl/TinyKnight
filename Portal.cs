using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Portal : Entity
    {
        public enum EnemyType
        {
            Walker, Flyer
        }

        public uint MaxEntities { get; init; }
        public double DelayBetweenSpawns { get; init; }

        private uint entitiesSpawned = 0;
        private double timer = 0f;

        private readonly EnemyType enemyType;
        private readonly bool showDebugInfo = false;

        public Portal(Vector2 position, Game game, EnemyType enemyType)
            : base(game, GetSprite(enemyType))
        {
            Position = position;
            this.enemyType = enemyType;
            this.sprite.LayerDepth = 1f;
            this.sprite.Scale = Vector2.One / 1.5f;
        }

        private static Sprite GetSprite(EnemyType enemyType)
        {
            var texture = enemyType switch
            {
                EnemyType.Flyer => Textures.PortalYellow,
                EnemyType.Walker => Textures.PortalOrange,
                _ => throw new System.ArgumentException($"Unsupported enemy type {enemyType}"),
            };
            return new Sprite(texture);
        }

        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            sprite.Rotation -= .025f;

            if (timer >= DelayBetweenSpawns && entitiesSpawned < MaxEntities)
            {
                timer = 0.0;

                Damageable enemy = enemyType switch
                {
                    EnemyType.Flyer => new Flyer(game),
                    EnemyType.Walker => new Walker(game),
                    _ => throw new System.ArgumentException($"Enemy type {enemyType} not supported!")
                };

                enemy.SetCoordinates(Position.X, Position.Y);
                enemy.OnDie += (_) => entitiesSpawned--;
                game.AddEntity(enemy);
                entitiesSpawned++;
            }
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
    }
}
