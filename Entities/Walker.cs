using Gravity.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gravity.Entities
{
    public class Walker : Damageable
    {
        private int facing;
        private bool dead = false;

        public Walker(GameplayScreen gameplayScreen)
            : base(gameplayScreen, health: 100)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;


            animator = new Animator(new List<Animation.Animation> { });

            facing = Numerics.PickOne(-1, 1);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (dead && normal == -Vector2.UnitY)
                ScheduleToDestroy();
        }

        public override void Update(GameTime gameTime)
        {
            const float speed = .05f;

            if (!dead && Level.HasCollision(CX, CY + 1))
                DX = Math.Sign(facing) * speed;

            if (!dead &&
                (Level.HasCollision(CX + 1, CY) && XR >= .7f ||
                Level.HasCollision(CX - 1, CY) && XR <= .3f))
            {
                facing = -facing;
                DX = Math.Sign(facing) * speed;
            }

            //if (facing > 0)
            //    //animator.Frame.Image.Flip = SpriteEffects.FlipHorizontally;
            //else
            //    //animator.Frame.Image.Flip = SpriteEffects.None;

            if (dead)
            {
                animator.Play("Walker_Dead");
                //animator.Frame.Image.Rotation += Random.FloatRange(
                //    MathHelper.PiOver4,
                //    MathHelper.PiOver2) * DX;
            }
        }

        public override void Die()
        {
            DY = Random.FloatRange(-.4f, -.5f);
            dead = true;
        }
    }
}
