﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Enemy : Entity
    {
        private readonly Spawner spawner;

        private int direction;

        public Enemy(Texture2D texture, Level level, Spawner spawner) : base(texture, level)
        {
            this.spawner = spawner;
            var rng = new Random();
            direction = rng.Next(0, 2) == 0 ? 1 : -1;
        }

        public override void Update(GameTime gameTime)
        {
            if (HasCollision(CX, CY + 1))
                DX = Math.Sign(direction) * .1f;

            if ((HasCollision(CX + 1, CY) && XR >= .7f) || (HasCollision(CX - 1, CY) && XR <= .3f))
            {
                direction = -direction;
                DX = Math.Sign(direction) * .1f;
            }

            if (level[CX, CY].Type == Cell.CellType.Water)
                SetCoordinates(spawner.Position.X, spawner.Position.Y);

            base.Update(gameTime);
        }
    }
}
