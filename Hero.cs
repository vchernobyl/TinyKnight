using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Hero : Entity
    {
        private bool onGround = false;

        public Hero(Game game, Texture2D texture, Level level)
            : base(game, texture, level)
        {
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .15f;
            var jump = -1f;

            if (Keyboard.IsKeyDown(Keys.Left))
                DX = -speed;
            if (Keyboard.IsKeyDown(Keys.Right))
                DX = speed;
            if (Keyboard.WasKeyPressed(Keys.Up) && onGround)
                DY = jump;

            onGround = level.HasCollision(CX, CY + 1);

            base.Update(gameTime);
        }
    }
}
