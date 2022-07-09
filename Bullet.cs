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
            if (other is Damageable enemy && enemy.IsAlive)
            {
                enemy.ReceiveDamage(Damage);
                IsActive = false;
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (normal == Vector2.UnitX || normal == -Vector2.UnitX)
                IsActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            DX = Velocity.X;
            DY = Velocity.Y;

            if (Velocity.X < 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;
        }
    }
}
