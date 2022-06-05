using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    public class Hero : Entity
    {
        public event Action? OnLevelCompleted;

        public Hero(Texture2D texture, Level level) : base(texture, level)
        {
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .15f;
            var jump = -.5f;

            if (Keyboard.IsKeyDown(Keys.Left))
                DX = -speed;
            if (Keyboard.IsKeyDown(Keys.Right))
                DX = speed;
            if (Keyboard.WasKeyPressed(Keys.Up))
                DY = jump;

            base.Update(gameTime);

            if (level.Cells[CX, CY].Type == Cell.CellType.Goal)
                OnLevelCompleted?.Invoke();
        }
    }
}
