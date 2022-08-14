using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }

        public Bullet(GameplayScreen gameplayScreen) : base(gameplayScreen, new Sprite(Textures.Bullet))
        {
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Damageable enemy && enemy.IsAlive)
            {
                enemy.ReceiveDamage(Damage);
                ScheduleToDestroy();
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (normal == Vector2.UnitX || normal == -Vector2.UnitX)
                ScheduleToDestroy();
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
