using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }

        public Bullet(Game game, Vector2 position, Vector2 velocity, int damage)
            : base(game, new Sprite(Textures.Bullet))
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is IDamageable enemy)
                enemy.ReceiveDamage(Damage);
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
