using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    public class Hero : Entity
    {
        public uint Coins { get; private set; }
        public uint EnemiesKilled { get; set; }
        public int Facing { get; private set; } = -1;

        private readonly Pistol pistol;

        private bool onGround = false;

        public Hero(Game game) : base(game, new Sprite(Textures.Hero))
        {
            pistol = new Pistol(game, this);
        }

        public void PickupCoin()
        {
            Coins++;
        }

        public void Knockback(float amount)
        {
            DX = -Facing * amount;
        }

        public override void OnEntityCollision(Entity other)
        {
            // TODO: Player death state.
            if (other is Enemy && false)
            {
                DY = -.3f;
                DX = Math.Sign(other.DX)* .3f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .15f;
            var jump = -1f;

            if (Input.IsKeyDown(Keys.Left))
            {
                sprite.Flip = SpriteEffects.None;
                DX = -speed;
                Facing = -1;
            }
            if (Input.IsKeyDown(Keys.Right))
            {
                sprite.Flip = SpriteEffects.FlipHorizontally;
                DX = speed;
                Facing = 1;
            }

            if (Input.WasKeyPressed(Keys.Up) && onGround)
            {
                DY = jump;
                SoundFX.HeroJump.Play(volume: .7f, 0f, 0f);
            }

            onGround = level.HasCollision(CX, CY + 1);

            base.Update(gameTime);

            pistol.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            pistol.Draw(batch);
        }
    }
}
