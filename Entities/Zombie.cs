using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Entities
{
    public class Zombie : Enemy
    {
        private int facing;
        private bool dead = false;
        private float rotationSpeed;

        private readonly int deadAnimID;

        public Zombie(GameplayScreen gameplayScreen)
            : base(gameplayScreen, health: 100, updateOrder: 200)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Zombie"));
            var walkAnim = spriteSheet.CreateAnimation("Zombie_Walk", out int walkAnimID);
            walkAnim.AddFrame(new Rectangle(0 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(1 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(2 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(3 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(4 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(5 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(6 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(7 * 8, 0, 8, 8), .1f);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Midground;
            sprite.Play(walkAnimID);

            var deadAnim = spriteSheet.CreateAnimation("Zombie_Dead", out deadAnimID);
            deadAnim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0f);

            facing = Numerics.PickOne(-1, 1);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (dead && normal == -Vector2.UnitY)
                Destroy();
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

            if (facing > 0)
                sprite.Flip = SpriteEffects.None;
            else
                sprite.Flip = SpriteEffects.FlipHorizontally;
        }

        public override void OnDie()
        {
            DY = Random.FloatRange(-.4f, -.5f);
            dead = true;
            sprite.Play(deadAnimID);
        }
    }
}
