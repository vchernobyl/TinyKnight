using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        // TODO: Feed the value via config file.
        public const int Damage = 40;

        public Vector2 Velocity;

        private readonly float spreadVariation = 0.025f;

        public Bullet(Game game, Vector2 position, Vector2 velocity)
            : base(game, new Sprite(Textures.Bullet))
        {
            SetCoordinates(position.X, position.Y);
            Velocity = velocity;
            Velocity.Y = Random.FloatRange(-spreadVariation, spreadVariation);
        }

        public override void Update(GameTime gameTime)
        {
            DX = Velocity.X;
            DY = Velocity.Y;

            if (Velocity.X < 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            if (level.HasCollision(CX + 1, CY) ||
                level.HasCollision(CX - 1, CY))
                IsActive = false;

            base.Update(gameTime);
        }
    }
}
