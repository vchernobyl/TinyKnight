using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Spawner
    {
        public Point Position { get; private set; }

        public Spawner(Point position, Game game)
        {
            Position = position;

            var enemy = new Enemy(game, game.Content.Load<Texture2D>("Textures/character_0015"), game.Level, this);
            enemy.SetCoordinates(position.X, position.Y);
            game.AddEntity(enemy);
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
