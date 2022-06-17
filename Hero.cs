using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Hero : Entity
    {
        private bool onGround = false;
        private int facing = -1;

        public Hero(Game game, Sprite sprite, Level level)
            : base(game, sprite, level)
        {
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .15f;
            var jump = -1f;

            if (Keyboard.IsKeyDown(Keys.Left))
            {
                sprite.Effect = SpriteEffects.None;
                DX = -speed;
                facing = -1;
            }
            if (Keyboard.IsKeyDown(Keys.Right))
            {
                sprite.Effect = SpriteEffects.FlipHorizontally;
                DX = speed;
                facing = 1;
            }

            if (Keyboard.WasKeyPressed(Keys.Up) && onGround)
                DY = jump;

            if (Keyboard.WasKeyPressed(Keys.Space))
            {
                var sprite = new Sprite(game.Content.Load<Texture2D>("Textures/bullet"));
                var bullet = new Bullet(game, sprite, level, new Vector2(XX, YY))
                {
                    Direction = facing,
                    Speed = 1f,
                };
                game.AddEntity(bullet);
            }

            onGround = level.HasCollision(CX, CY + 1);

            base.Update(gameTime);
        }
    }
}
