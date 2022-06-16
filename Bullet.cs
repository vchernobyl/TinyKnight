using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        public Vector2 Direction { get; init; }
        public float Speed { get; init; }

        public Bullet(Game game, Texture2D texture, Level level, Vector2 position)
            : base(game, texture, level)
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

        public override void OnCollision(Entity other)
        {
            if (other is Enemy)
            {
                IsActive = false;
                other.IsActive = false;
            }
        }
    }
}
