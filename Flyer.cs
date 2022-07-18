using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Gravity
{
    public class Flyer : Damageable, IEnemy
    {
        private readonly Pathfinding pathfinding;
        private readonly NavigationGrid navGrid;
        private readonly Timer pathfindingTimer;
        private readonly bool showNavigation = false;

        private List<Vector2> path = new List<Vector2>();
        private int pointIndex = 0;
        private bool dead = false;

        public Flyer(GameplayScreen gameplayScreen) : base(gameplayScreen, new Sprite(Textures.Flyer), health: 100)
        {
            Gravity = 0f;

            pathfindingTimer = new Timer(duration: 1.5, RecalculatePath, repeating: true, immediate: true);
            pathfindingTimer.Start();

            navGrid = new NavigationGrid(Level.Columns, Level.Rows);
            foreach (var cell in Level.Cells)
            {
                if (cell.Solid)
                    navGrid.Solids.Add(cell.Location);
            }

            foreach (var solid in navGrid.Solids)
            {
                foreach (var direction in NavigationGrid.Directions)
                {
                    var neighbour = new Point(solid.X + direction.X, solid.Y + direction.Y);
                    if (navGrid.InBounds(neighbour) &&
                        navGrid.Passable(neighbour))
                        navGrid.NearSolids.Add(neighbour);
                }
            }

            pathfinding = new Pathfinding(navGrid);
        }

        private void RecalculatePath()
        {
            pointIndex = 0;
            path = pathfinding.FindPath(Position, gameplayScreen.Hero.Position);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (dead && normal == -Vector2.UnitY)
                IsActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (dead)
            {
                sprite.Rotation += .3f;
                return;
            }

            pathfindingTimer.Update(gameTime);

            if (path.Count > 0 && pointIndex < path.Count)
            {
                var currentPoint = path[pointIndex];

                // Move to the towards next waypoint if close enough to the current target waypoint.
                if (Vector2.Distance(Position, currentPoint) < 10f)
                    pointIndex++;

                var movement = currentPoint - Position;
                if (movement != Vector2.Zero)
                    movement.Normalize();

                DX += movement.X * .005f;
                DY += movement.Y * .005f;
            }
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

                navGrid.Draw(batch);
            }

            base.Draw(batch);
        }

        public override void Die()
        {
            gameplayScreen.Hero.EnemiesKilled++;
            dead = true;
            DY = -.5f;
            Gravity = .05f;
        }
    }
}
