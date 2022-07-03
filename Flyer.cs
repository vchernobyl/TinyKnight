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
        private int pointIndex = 0;
        private double timer = 0;

        public Flyer(Game game) : base(game, new Sprite(Textures.Flyer))
        {
            hero = game.Hero;
            pathfinding = new Pathfinding(level.Grid);
        }

        public override void Update(GameTime gameTime)
        {
            ///timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (path.Count == 0 || timer >= .15)
            {
                timer = 0;
                pointIndex = 0;
                path = pathfinding.FindPath(new Point(CX, CY), new Point(hero.CX, hero.CY));
            }
    
            if (path.Count > 0 && pointIndex < path.Count)
            {
                var currentPoint = path[pointIndex];
                if (currentPoint == new Point(CX, CY))
                    pointIndex++;

                var movement = currentPoint - new Point(CX, CY);
                movement.ToVector2().Normalize();
                DX = movement.X * .05f;
                DY = movement.Y * .05f;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            foreach (var node in path)
            {
                var position = new Point(node.X * Level.CellSize, node.Y * Level.CellSize);
                var size = new Point(Level.CellSize, Level.CellSize);
                var rect = new Rectangle(position, size);
                rect.Inflate(-10f, -10f);
                batch.DrawRectangle(rect, Color.LawnGreen);
            }

            base.Draw(batch);
        }
    }
}

