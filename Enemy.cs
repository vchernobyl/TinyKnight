using Microsoft.Xna.Framework;
using System;

namespace Gravity
{
    public class Enemy : Entity
    {
        public event Action<Enemy>? OnDie;

        private readonly Spawner spawner;
        private int direction;

        public Enemy(Game game, Sprite sprite, Level level, Spawner spawner)
            : base(game, sprite, level)
        {
            this.spawner = spawner;
            var rng = new Random();
            direction = rng.Next(0, 2) == 0 ? 1 : -1;
        }

        public override void Update(GameTime gameTime)
        {
            if (level.HasCollision(CX, CY + 1))
                DX = Math.Sign(direction) * .1f;

            if ((level.HasCollision(CX + 1, CY) && XR >= .7f) || (level.HasCollision(CX - 1, CY) && XR <= .3f))
            {
                direction = -direction;
                DX = Math.Sign(direction) * .1f;
            }

            if (level[CX, CY].Type == Cell.CellType.Water)
                SetCoordinates(spawner.Position.X, spawner.Position.Y);

            base.Update(gameTime);
        }

        public override void OnDestroy()
        {
            OnDie?.Invoke(this);
        }
    }
}
