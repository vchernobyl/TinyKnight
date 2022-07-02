using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Gravity
{
    public class Flyer : Entity
    {
        private readonly Hero hero;
        private readonly Pathfinding pathfinding;

        private List<Point> path = new();

        public Flyer(Game game) : base(game, new Sprite(Textures.Flyer))
        {
            hero = game.Hero;
            Position = new Vector2(300f, 200f);
            pathfinding = new Pathfinding(level.Grid);
        }

        public override void Update(GameTime gameTime)
        {
            path = pathfinding.FindPath(new Point(CX, CY), new Point(hero.CX, hero.CY));
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            foreach (var node in path)
            {
                var position = new Point(node.X * Level.CellSize, node.Y * Level.CellSize);
                var size = new Point(Level.CellSize, Level.CellSize);
                var rect = new Rectangle(position, size);
                rect.Inflate(-9f, -9f);
                batch.DrawRectangle(rect, Color.LawnGreen);
            }

            base.Draw(batch);
        }
    }
}

