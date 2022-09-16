using Gravity.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Arrow : Entity
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }

        public Arrow(GameplayScreen gameplayScreen) 
            : base(gameplayScreen)
        {
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Enemy enemy && enemy.IsAlive)
            {
                enemy.Damage(Damage);
                Destroy();
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (normal == Vector2.UnitX || normal == -Vector2.UnitX)
                Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            DX = Velocity.X;
            DY = Velocity.Y;

            //if (Velocity.X < 0)
            //    sprite.Flip = SpriteEffects.FlipHorizontally;
            //else
            //    sprite.Flip = SpriteEffects.None;
        }
    }
}
