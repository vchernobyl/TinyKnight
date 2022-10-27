using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Entities
{
    public class Demon : Enemy
    {
        private int facing;
        private bool dead;

        public Demon(GameplayScreen gameplayScreen)
            : base(gameplayScreen, health: 100)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;

            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Demon"));
            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Midground;
            sprite.Play(animID);

            facing = Numerics.PickOne(-1, 1);
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            if (dead && normal == -Vector2.UnitY)
                Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            const float speed = .1f;
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
        }
    }
}
