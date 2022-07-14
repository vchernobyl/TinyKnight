using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Pellet : Entity
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }

        private uint collisions = 0;

        public Pellet(GameplayScreen gameplayScreen, Vector2 position, Vector2 velocity, int damage)
            : base(gameplayScreen, new Sprite(Textures.Bullet))
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Collision = true;
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
            Velocity = Vector2.Reflect(Velocity, normal);

            if (collisions++ == 4)
                IsActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            DX = Velocity.X;
            DY = Velocity.Y;
            sprite.Rotation = Numerics.VectorToRadians(new Vector2(DX, DY));
        }

        public override void Draw(SpriteBatch batch)
        {
            DX = Velocity.X;

            if (Velocity.X < 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            base.Draw(batch);
        }
    }
}
