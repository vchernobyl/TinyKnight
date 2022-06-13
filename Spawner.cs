using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Spawner
    {
        private readonly Point position;
        private readonly Game game;

        public Spawner(Point position, Game game)
        {
            this.position = position;
            this.game = game;

            var enemy = new Enemy(game.Content.Load<Texture2D>("Textures/character_0015"), game.Level);
            enemy.SetCoordinates(position.X, position.Y);
            game.AddEntity(enemy);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch batch)
        {
            var outline = new Rectangle(position.X, position.Y, Level.CellSize, Level.CellSize);
            batch.DrawRectangleOutline(outline, Color.Yellow);
        }
    }
}
