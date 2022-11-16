using Gravity.Entities;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Arrow : Entity
    {
        private readonly Vector2 velocity;
        private readonly int damage;

        public Arrow(GameplayScreen gameplayScreen, Vector2 velocity, int damage)
            : base(gameplayScreen)
        {
            this.velocity = velocity;
            this.damage = damage;

            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(8 * 1, 0, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Rotation = velocity.X < 0
                ? MathHelper.ToRadians(-135f)
                : MathHelper.ToRadians(45f);
            Sprite.Play(defaultAnimID);
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy && enemy.IsAlive)
            {
                enemy.Damage(damage);
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
            DX = velocity.X;
            DY = velocity.Y;
        }
    }
}
