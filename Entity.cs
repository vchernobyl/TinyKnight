using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Entity
    {
        protected readonly Game game;
        protected readonly Sprite sprite;
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

        public readonly float Radius = Level.CellSize / 2;

        public bool IsActive = true;
        public bool Collision = true;

        public Vector2 Position
        {
            get => new(XX, YY);
            set => SetCoordinates(value.X, value.Y);
        }

        public bool IsFlashing => flashTime > .0;

        private double flashTime = .0;

        public Entity(Game game, Sprite sprite)
        {
            this.game = game;
            this.sprite = sprite;
            this.level = game.Level;
        }

        public void SetCoordinates(float x, float y)
        {
            XX = x;
            YY = y;
            CX = (int)(XX / Level.CellSize);
            CY = (int)(YY / Level.CellSize);
            XR = (XX - CX * Level.CellSize) / Level.CellSize;
            YR = (YY - CY * Level.CellSize) / Level.CellSize;
            sprite.Position = Position;
        }

        public bool Overlaps(Entity other)
        {
            var maxDist = Radius + other.Radius;
            var distSqr = (other.XX - XX) * (other.XX - XX) + (other.YY - YY) * (other.YY - YY);
            return distSqr <= maxDist * maxDist;
        }

        public void Flash(double duration)
        {
            flashTime = duration;
        }

        public virtual void OnEntityCollision(Entity other) { }

        public virtual void OnLevelCollision(Vector2 normal) { }

        public virtual void OnDestroy() { }

        public virtual void Update(GameTime gameTime)
        {
            flashTime = Math.Max(.0, flashTime - gameTime.ElapsedGameTime.TotalSeconds);

            // Check for collisions with other entities.
            if (Collision)
            {
                foreach (var other in game.Entities)
                {
                    if (this != other && Overlaps(other))
                        OnEntityCollision(other);
                }
            }

            XR += DX;
            DX *= .9f;

            // Right side collision.
            if (level.HasCollision(CX + 1, CY) && XR >= .7f)
            {
                XR = .7f;
                DX = 0f;
                OnLevelCollision(-Vector2.UnitX);
            }

            // Left side collision.
            if (level.HasCollision(CX - 1, CY) && XR <= .3f)
            {
                XR = .3f;
                DX = 0f;
                OnLevelCollision(Vector2.UnitX);
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
                OnLevelCollision(Vector2.UnitY);
            }

            // Bottom collision.
            if (level.HasCollision(CX, CY + 1) && YR >= .5f)
            {
                DY = 0f;
                YR = .5f;
                OnLevelCollision(-Vector2.UnitY);
            }

            while (YR > 1) { CY++; YR--; }
            while (YR < 0) { CY--; YR++; }

            XX = (int)((CX + XR) * Level.CellSize);
            YY = (int)((CY + YR) * Level.CellSize);
            sprite.Position = Position;
        }

        public virtual void Draw(SpriteBatch batch)
        {
            sprite.Draw(batch);
        }
    }
}
