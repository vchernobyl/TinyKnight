using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Entity
    {
        protected readonly Game game;
        protected readonly Texture2D texture;
        protected readonly Level level;

        // Coordinates within the grid.
        public int CX = 0;
        public int CY = 0;

        // Position within the cell itself, within range (0, 1).
        public float XR = .5f;
        public float YR = 1f;

        // Resulting coordinates.
        public float XX, YY;

        // Movement.
        public float DX = 0f;
        public float DY = 0f;

        public readonly float Radius = 12f;

        public bool EntityCollision { get; init; } = false;

        public Entity(Game game, Texture2D texture, Level level)
        {
            this.game = game;
            this.texture = texture;
            this.level = level;
        }

        public void SetCoordinates(float x, float y)
        {
            XX = x;
            YY = y;
            CX = (int)(XX / Level.CellSize);
            CY = (int)(YY / Level.CellSize);
            XR = (XX - CX * Level.CellSize) / Level.CellSize;
            YR = (YY - CY * Level.CellSize) / Level.CellSize;
        }

        public bool Overlaps(Entity other)
        {
            var maxDist = Radius + other.Radius;
            var distSqr = (other.XX - XX) * (other.XX - XX) + (other.YY - YY) * (other.YY - YY);
            return distSqr <= maxDist * maxDist;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Check for collisions with other entities.
            if (EntityCollision)
            {
                foreach (var other in game.Entities)
                {
                    // Check if entities are close enough.
                    if (other != this &&
                        Math.Abs(CX - other.CX) <= 2 &&
                        Math.Abs(CY - other.CY) <= 2)
                    {
                        var dist = Math.Sqrt((other.XX - XX) * (other.XX - XX) + (other.YY - YY) * (other.YY - YY));
                        if (dist <= Radius + other.Radius)
                        {
                            var angle = Math.Atan2(other.YY - YY, other.XX - XX);
                            var force = .2f;
                            var repelPower = (Radius + other.Radius - dist) / (Radius + other.Radius);
                            DX -= (float)(Math.Cos(angle) * repelPower * force);
                            DY -= (float)(Math.Sin(angle) * repelPower * force);
                            other.DX += (float)(Math.Cos(angle) * repelPower * force);
                            other.DY += (float)(Math.Sin(angle) * repelPower * force);
                        }
                    }
                }
            }

            XR += DX;
            DX *= .9f;

            // Right side collision.
            if (level.HasCollision(CX + 1, CY) && XR >= .7f)
            {
                XR = .7f;
                DX = 0f;
            }

            // Left side collision.
            if (level.HasCollision(CX - 1, CY) && XR <= .3f)
            {
                XR = .3f;
                DX = 0f;
            }

            while (XR > 1) { XR--; CX++; }
            while (XR < 0) { XR++; CX--; }

            YR += DY;
            DY += .05f;
            DY *= .9f;

            // Top collision.
            if (level.HasCollision(CX, CY - 1) && YR <= .3f)
            {
                DY = .05f;
                YR = .3f;
            }

            // Bottom collision.
            if (level.HasCollision(CX, CY + 1) && YR >= .5f)
            {
                DY = 0f;
                YR = .5f;
            }

            while (YR > 1) { CY++; YR--; }
            while (YR < 0) { CY--; YR++; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            XX = (int)((CX + XR) * Level.CellSize);
            YY = (int)((CY + YR) * Level.CellSize);

            // Draw the sprite at the center of the entity position, not the top-left corner.
            var dest = new Rectangle(
                (int)(XX - Level.CellSize / 2), (int)(YY - Level.CellSize / 2),
                Level.CellSize, Level.CellSize);

            spriteBatch.Draw(texture, dest, Color.White);
        }
    }
}
