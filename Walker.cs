using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Walker : Damageable
    {
        private int facing;
        private bool dead = false;

        public Walker(GameplayScreen gameplayScreen)
            : base(gameplayScreen, new Sprite(Textures.Enemy), health: 100)
        {
            facing = Numerics.PickOne(-1, 1);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (dead && normal == -Vector2.UnitY)
                ScheduleToDestroy();
        }

        public override void Update(GameTime gameTime)
        {
            if (!dead && Level.HasCollision(CX, CY + 1))
                DX = Math.Sign(facing) * .1f;

            if (!dead &&
                ((Level.HasCollision(CX + 1, CY) && XR >= .7f) ||
                (Level.HasCollision(CX - 1, CY) && XR <= .3f)))
            {
                facing = -facing;
                DX = Math.Sign(facing) * .1f;
            }

            if (facing > 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            if (dead)
            {
                sprite.Rotation += Random.FloatRange(
                    MathHelper.PiOver4,
                    MathHelper.PiOver2) * DX;
            }
        }

        public override void Die()
        {
            gameplayScreen.Hero.EnemiesKilled++;

            DY = Random.FloatRange(-.4f, -.5f);
            dead = true;
        }
    }
}
