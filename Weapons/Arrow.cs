using Gravity.Entities;
using Gravity.Graphics;
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
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(8, 0, 8, 8), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
        }

        public override void OnEntityCollisionEnter(Entity other)
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

            if (Velocity.X < 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;
        }
    }
}
