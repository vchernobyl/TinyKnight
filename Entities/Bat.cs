using Gravity.AI;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Gravity.Entities
{
    public class Bat : Enemy
    {
        private readonly Pathfinding pathfinding;
        private readonly NavigationGrid navGrid;
        private readonly Timer pathfindingTimer;

        private List<Vector2> path = new List<Vector2>();
        private int pointIndex = 0;

        public Bat(GameplayScreen gameplayScreen) : base(gameplayScreen, health: 100)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Bat"));
            var anim = spriteSheet.CreateAnimation("Bat_Fly", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), .1f);
            anim.AddFrame(new Rectangle(8, 0, 8, 8), .1f);
            anim.AddFrame(new Rectangle(16, 0, 8, 8), .1f);
            anim.AddFrame(new Rectangle(24, 0, 8, 8), .1f);

            Sprite = spriteSheet.Create();
            Sprite.Play(animID);

            Gravity = 0f;

            pathfindingTimer = new Timer(duration: 1.5f, RecalculatePath, repeating: true, immediate: true);
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
            path = pathfinding.FindPath(Position, GameplayScreen.Hero.Position);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (!IsAlive && normal == -Vector2.UnitY)
                Destroy();
        }

        public override void Update(GameTime gameTime)
        {
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

                if (DX >= 0)
                    Sprite.Flip = SpriteEffects.None;
                else
                    Sprite.Flip = SpriteEffects.FlipHorizontally;
            }
        }

        public override void OnDie()
        {
            Gravity = .05f;
        }
    }
}
