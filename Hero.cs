using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    public class Hero : Entity
    {
        public uint EnemiesKilled { get; set; }
        public int Facing { get; private set; } = -1;
        public Weapon CurrentWeapon { get; set; }

        private readonly Weapons weapons;
        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;
        private float lockDuration = 0f;
        private float lockTime = 0f;
        private Portal portal;

        public bool IsLocked => lockDuration > 0f;

        public Hero(GameplayScreen gamplayScreen) : base(gamplayScreen, new Sprite(Textures.Hero))
        {
            weapons = new Weapons(gamplayScreen, this);
            CurrentWeapon = weapons.Bazooka;
        }

        public void Knockback(float amount)
        {
            DX += -Facing * amount;
        }

        public override void OnEntityCollision(Entity other)
        {
            // TODO: Player death state.
            if (other is Damageable enemy && enemy.IsAlive && !hurting)
            {
                hurting = true;
                hurtTime = .2;
                SoundFX.HeroHurt.Play();
                Flash(duration: .15f, Color.Red);

                DY = -.3f;
                DX = Math.Sign(XX - other.XX) * .3f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (lockDuration > 0f)
            {
                lockTime += gameTime.DeltaTime();
                if (lockTime >= lockDuration)
                {
                    lockTime = 0f;
                    lockDuration = 0f;
                    Gravity = .05f;

                    // Get new weapon and spawn weapon text.
                    CurrentWeapon = weapons.GetRandomWeapon(CurrentWeapon);
                    var game = gameplayScreen.ScreenManager.Game;
                    game.Components.Add(new WeaponPickupText(game, CurrentWeapon.Name, Position));

                    foreach (var e in gameplayScreen.Entities)
                    {
                        if (e != this && e != portal)
                            e.EntityState = State.Active;
                    }
                }
                else
                {
                    Position = Vector2.Lerp(Position, portal.Position, lockTime / lockDuration);
                    return;
                }
            }

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
            {
                CurrentWeapon = weapons.Pistol;
            }
            if (Input.WasKeyPressed(Keys.D2))
            {
                CurrentWeapon = weapons.Shotgun;
            }
            if (Input.WasKeyPressed(Keys.D3))
            {
                CurrentWeapon = weapons.Bazooka;
            }

            onGround = Level.IsWithinBounds(CX, CY) && Level.HasCollision(CX, CY + 1);

            if (Input.IsKeyDown(Keys.Space))
                CurrentWeapon.PullTrigger();

            CurrentWeapon.Update(gameTime);
        }

        public void Lock(float duration, Portal portal)
        {
            this.lockDuration = duration;
            this.lockTime = 0f;
            this.portal = portal;

            foreach (var e in gameplayScreen.Entities)
            {
                if (e != this && e != portal)
                    e.EntityState = State.Paused;
            }
        }
    }
}
