using TinyKnight.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyKnight.AI
{
    public interface ICommand
    {
        void Execute(Entity ent);
    }

    public class AIBehaviour
    {
        public bool Enabled { get; set; } = true;

        private readonly ICommand command;

        public AIBehaviour(ICommand command)
        {
            this.command = command;
        }

        public ICommand? Update(GameTime gameTime)
        {
            return Enabled ? command : null;
        }
    }

    public class WalkCommand : ICommand
    {
        public float Speed { get; set; } = .1f;

        private int facing = Numerics.PickOne(-1, 1);

        public void Execute(Entity en)
        {
            var enemy = (Enemy)en;

            if (enemy.IsAlive && enemy.Level.HasCollision(enemy.CX, enemy.CY + 1))
                enemy.DX = Math.Sign(facing) * Speed;

            if (enemy.IsAlive &&
                (enemy.Level.HasCollision(enemy.CX + 1, enemy.CY) && enemy.XR >= .7f ||
                enemy.Level.HasCollision(enemy.CX - 1, enemy.CY) && enemy.XR <= .3f))
            {
                facing = -facing;
                enemy.DX = Math.Sign(facing) * Speed;
            }

            if (facing > 0)
                enemy.Sprite.Flip = SpriteEffects.None;
            else
                enemy.Sprite.Flip = SpriteEffects.FlipHorizontally;
        }
    }
}
