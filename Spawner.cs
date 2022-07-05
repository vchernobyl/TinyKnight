using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Spawner
    {
        public enum EnemyType
        {
            Walker, Flyer
        }

        public Vector2 Position { get; private set; }

        public uint MaxEntities { get; init; }
        public double DelayBetweenSpawns { get; init; }

        private uint entitiesSpawned = 0;
        private double timer = 0f;

        private readonly Game game;
        private readonly EnemyType enemyType;

        public Spawner(Vector2 position, Game game, EnemyType enemyType)
        {
            Position = position;
            this.game = game;
            this.enemyType = enemyType;
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

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

        public void Draw(SpriteBatch batch)
        {
            var outline = new Rectangle(
                (int)Position.X - Level.CellSize / 2,
                (int)Position.Y - Level.CellSize / 2,
                Level.CellSize,
                Level.CellSize);
            batch.DrawRectangleOutline(outline, Color.Yellow, 2f);
        }
    }
}
