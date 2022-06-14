using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Hero : Entity
    {
        private bool onGround = false;

        public Hero(Texture2D texture, Level level) : base(texture, level)
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

            onGround = HasCollision(CX, CY + 1);

            if (level.Cells[CX, CY].Type == Cell.CellType.Water)
                SetCoordinates(50f, 100f);

            base.Update(gameTime);
        }
    }
}
