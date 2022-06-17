using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        public int Direction { get; init; }
        public float Speed { get; init; }

        public Bullet(Game game, Sprite sprite, Level level, Vector2 position)
            : base(game, sprite, level)
        {
            SetCoordinates(position.X, position.Y);
        }

        public override void Update(GameTime gameTime)
        {
            DX = Direction * Speed;
            DY = 0f;

            if (Direction < 0)
                sprite.Effect = SpriteEffects.FlipHorizontally;
            else
                sprite.Effect = SpriteEffects.None;

            if (level.HasCollision(CX + 1, CY) ||
                level.HasCollision(CX - 1, CY))
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
