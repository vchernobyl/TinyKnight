using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Bullet : Entity
    {
        public Vector2 Direction { get; init; }
        public float Speed { get; init; }

        public Bullet(Game game, Sprite sprite, Level level, Vector2 position)
            : base(game, sprite, level)
        {
            SetCoordinates(position.X, position.Y);
        }

        public override void Update(GameTime gameTime)
        {
            DX = Direction.X * Speed;
            DY = Direction.Y * Speed;

            if (level.HasCollision(CX + 1, CY))
                IsActive = false;

            base.Update(gameTime);
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Enemy)
            {
                IsActive = false;
                other.IsActive = false;
            }
        }
    }
}
