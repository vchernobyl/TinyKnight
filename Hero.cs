using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Gravity
{
    public class Hero : Entity
    {
        private bool jumping = false;

        public Hero(Texture2D texture, Level grid) : base(texture, grid)
        {
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .15f;
            var jump = -.5f;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                DX = -speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                DX = speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !jumping)
            {
                DY = jump;
                jumping = true;
            }

            if (HasCollision(CX, CY + 1) && jumping)
            {
                Debug.WriteLine("landed!");
                jumping = false;
            }

            base.Update(gameTime);

            if (grid.Cells[CX, CY].Type == Cell.CellType.Goal)
            {
                Debug.WriteLine("Goal reached!");
            }
        }
    }
}
