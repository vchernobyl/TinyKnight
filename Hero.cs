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

        private readonly IWeapon Pistol;
        private readonly IWeapon Shotgun;

        private IWeapon weapon;
        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;

        public Hero(Game game) : base(game, new Sprite(Textures.Hero))
        {
            Pistol = new Pistol(game, this);
            Shotgun = new Shotgun(game, this);
            weapon = Pistol;
        }

        public void PickupCoin()
        {
            Coins++;
        }

        public void Knockback(float amount)
        {
            DX += -Facing * amount;
        }

        public override void OnEntityCollision(Entity other)
        {
            // TODO: Player death state.
            if (other is IEnemy && !hurting)
            {
                hurting = true;
                hurtTime = .35;
                SoundFX.HeroHurt.Play();
                Flash(duration: .15f, new Vector4(1f, 0f, 0f, 1f));

                DY = -.3f;
                DX = Math.Sign(XX - other.XX) * .3f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .0175f;
            var jump = -1f;

            hurtTime = Math.Max(0, hurtTime - gameTime.ElapsedGameTime.TotalSeconds);
            if (hurtTime == 0)
                hurting = false;

            // Movement.
            if (!hurting)
            {
                if (Input.IsKeyDown(Keys.Left))
                {
                    sprite.Flip = SpriteEffects.None;
                    DX += -speed;
                    Facing = -1;
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    sprite.Flip = SpriteEffects.FlipHorizontally;
                    DX += speed;
                    Facing = 1;
                }
                if (Input.WasKeyPressed(Keys.Up) && onGround)
                {
                    DY = jump;
                    SoundFX.HeroJump.Play(volume: .7f, 0f, 0f);
                }
            }

            // Weapon switching.
            if (Input.WasKeyPressed(Keys.D1))
                weapon = Pistol;
            if (Input.WasKeyPressed(Keys.D2))
                weapon = Shotgun;

            onGround = level.HasCollision(CX, CY + 1);

            base.Update(gameTime);

            weapon.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            weapon.Draw(batch);
        }
    }
}
