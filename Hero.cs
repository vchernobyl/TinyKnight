using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Gravity
{
    public enum HeroState
    {
        Idle,
        Running,
        Jumping,
    }

    public class Hero : Entity
    {
        public uint EnemiesKilled { get; set; }
        public int Facing { get; private set; } = -1;
        public Weapon CurrentWeapon { get; set; }

        public bool IsLocked => lockDuration > 0f;
        
        private readonly Weapons weapons;
        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;
        private float lockDuration = 0f;
        private float lockTime = 0f;
        private Portal portal;
        private HeroState state = HeroState.Idle;

        public Hero(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            weapons = new Weapons(gameplayScreen, this);
            CurrentWeapon = weapons.Bazooka;

            var content = base.gameplayScreen.ScreenManager.Game.Content;


            var sprite1 = new Sprite(content.Load<Texture2D>("Textures/Tiny_Knight"));
            var sprite2 = new Sprite(content.Load<Texture2D>("Textures/Tiny_Knight"));

            var runFrames = new List<Animation.Frame>
            {
                new Animation.Frame(sprite1, .15f),
                new Animation.Frame(sprite2, .15f)
            };

            var idleFrames = new List<Animation.Frame>
            {
                new Animation.Frame(sprite1, .15f)
            };

            var animations = new List<Animation>
            {
                new Animation("Hero_Run", runFrames),
                new Animation("Hero_Idle", idleFrames)
            };

            animator = new Animator(animations);
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
                    animator.Frame.Sprite.Flip = SpriteEffects.None;
                    DX += -speed;
                    Facing = -1;
                    state = HeroState.Running;
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    animator.Frame.Sprite.Flip = SpriteEffects.FlipHorizontally;
                    DX += speed;
                    Facing = 1;
                    state = HeroState.Running;
                }
                if (Input.WasKeyPressed(Keys.Up) && onGround)
                {
                    DY = jump;
                    SoundFX.HeroJump.Play(volume: .7f, 0f, 0f);
                    state = HeroState.Jumping;
                }
            }

            if (MathF.Abs(DX) < .01f)
                state = HeroState.Idle;
            else
                state = HeroState.Running;

            if (state == HeroState.Idle)
                animator?.Play("Hero_Idle");
            else if (state == HeroState.Running)
                animator?.Play("Hero_Run");

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
