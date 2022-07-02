using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Spawner
    {
        public Point Position { get; private set; }

        public uint MaxEntities { get; init; }
        public double DelayBetweenSpawns { get; init; }

        private uint entitiesSpawned = 0;
        private double timer = 0f;

        private readonly Game game;

        public Spawner(Point position, Game game)
        {
            Position = position;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= DelayBetweenSpawns && entitiesSpawned < MaxEntities)
            {
                timer = 0.0;
                var enemy = new Enemy(game, this);
                enemy.SetCoordinates(Position.X, Position.Y);
                enemy.OnDie += (_) => entitiesSpawned--;
                game.AddEntity(enemy);
                entitiesSpawned++;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            var outline = new Rectangle(Position.X - Level.CellSize / 2,
                Position.Y - Level.CellSize / 2,
                Level.CellSize,
                Level.CellSize);
            batch.DrawRectangleOutline(outline, Color.Yellow, 2f);
        }
    }
}
