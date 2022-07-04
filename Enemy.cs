using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Enemy : Damageable
    {
        private readonly Spawner spawner;

        private int facing;
        private double deathTimer = 2.0;
        private bool startDeathAnimation = false;

        public Enemy(Game game, Spawner spawner)
            : base(game, new Sprite(Textures.Enemy), health: 100)
        {
            this.spawner = spawner;
            facing = Numerics.PickOne(-1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (!startDeathAnimation && level.HasCollision(CX, CY + 1))
                DX = Math.Sign(facing) * .1f;

            if (!startDeathAnimation &&
                ((level.HasCollision(CX + 1, CY) && XR >= .7f) ||
                (level.HasCollision(CX - 1, CY) && XR <= .3f)))
            {
                facing = -facing;
                DX = Math.Sign(facing) * .1f;
            }

            if (facing > 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            if (!startDeathAnimation &&
                level.IsWithinBounds(CX, CY) &&
                level[CX, CY].Type == Cell.CellType.Water)
            {
                SetCoordinates(spawner.Position.X, spawner.Position.Y);
            }

            if (startDeathAnimation)
            {
                deathTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                sprite.Rotation += Random.FloatRange(
                    MathHelper.PiOver4,
                    MathHelper.PiOver2) * DX;

                if (deathTimer <= 0f)
                    IsActive = false;
            }

            base.Update(gameTime);
        }

        public override void Die()
        {
            game.Hero.EnemiesKilled++;

            DY = Random.FloatRange(-.4f, -.5f);
            startDeathAnimation = true;
            Collision = false;

            game.AddEntity(new Coin(game) { Position = Position });
        }
    }
}
