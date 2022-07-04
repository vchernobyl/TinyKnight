using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Gravity
{
    public class Flyer : Damageable
    {
        private readonly Hero hero;
        private readonly Pathfinding pathfinding;
        private readonly bool showNavigation = false;

        private List<Vector2> path = new();
        private int pointIndex = 0;
        private double timer = 0;

        public Flyer(Game game) : base(game, new Sprite(Textures.Flyer), health: 100)
        {
            Gravity = 0f;
            hero = game.Hero;
            pathfinding = new Pathfinding(level.Grid);
        }

        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (path.Count == 0 || timer >= 1.5)
            {
                timer = 0;
                pointIndex = 0;
                path = pathfinding.FindPath(Position, hero.Position);
            }

            if (path.Count > 0 && pointIndex < path.Count)
            {
                var currentPoint = path[pointIndex];

                // Move to the towards next waypoint if close enough to the current target waypoint.
                if (Vector2.Distance(Position, currentPoint) < 10f)
                    pointIndex++;

                var movement = currentPoint - Position;
                if (movement != Vector2.Zero)
                    movement.Normalize();

                DX = movement.X * .05f;
                DY = movement.Y * .05f;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            if (showNavigation)
            {
                for (int i = 0; i < path.Count - 1; i++)
                    batch.DrawLine(path[i], path[i + 1], Color.DarkBlue, thickness: 2f);

                foreach (var point in path)
                {
                    var rectangle = new Rectangle(
                        (int)point.X - Level.CellSize / 2,
                        (int)point.Y - Level.CellSize / 2,
                        Level.CellSize,
                        Level.CellSize);
                    rectangle.Inflate(-9.5f, -9.5f);
                    batch.DrawRectangle(rectangle, Color.Blue);
                }
            }

            base.Draw(batch);
        }

        public override void Die()
        {
            game.Hero.EnemiesKilled++;
            IsActive = false;
        }
    }
}
