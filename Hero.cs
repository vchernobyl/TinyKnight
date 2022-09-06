using Gravity.Animation;
using Gravity.Particles;
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
        public int Facing { get; private set; }
        public Weapon CurrentWeapon { get; set; }
        public int Health { get; private set; }
        public const int MaxHealth = 3;

        private readonly Weapons weapons;
        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;
        private HeroState state = HeroState.Idle;

        private readonly ParticleSystem jumpParticles;
        private readonly ParticleSystem landingParticles;
        private readonly ParticleSystem runTrailParticles;

        private const float TrailParticleInterval = .145f;
        private float trailParticleTime = 0f;
        private float squashTime = 0f;

        public Hero(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            weapons = new Weapons(gameplayScreen, this);
            CurrentWeapon = weapons.Bazooka;
            Health = 3;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var idleSheet = content.Load<Texture2D>("Textures/Hero_Idle");
            var runSheet = content.Load<Texture2D>("Textures/Hero_Run");
            var animations = new List<Animation.Animation>
            {
                new Animation.Animation("Hero_Idle", idleSheet),
                new Animation.Animation("Hero_Run", runSheet),
            };

            animator = new Animator(animations);

            jumpParticles = new ParticleSystem(game, "Particles/HeroJumpParticleSettings");
            game.Components.Add(jumpParticles);

            landingParticles = new ParticleSystem(game, "Particles/HeroLandingParticleSettings");
            game.Components.Add(landingParticles);

            runTrailParticles = new ParticleSystem(game, "Particles/HeroRunTrailParticleSettings");
            game.Components.Add(runTrailParticles);
        }

        // TODO: Will be useful for other entities as well, for example enemies.
        public void Knockback(float amount)
        {
            DX += -Facing * amount;
        }

        public override void OnEntityCollision(Entity other)
        {
            // TODO: Player death state.
            if (other is Damageable enemy && enemy.IsAlive && !hurting)
            {
                Health--;
                if (Health <= 0)
                    gameplayScreen.ScreenManager.RemoveScreen(gameplayScreen);

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
            var speed = .0175f;
            var jump = -1.25f;

            trailParticleTime += gameTime.DeltaTime();

            hurtTime = Math.Max(0, hurtTime - gameTime.ElapsedGameTime.TotalSeconds);
            if (hurtTime == 0)
                hurting = false;

            // Squash.
            // TODO: This type of functionality might be quite common for entities in general,
            // for example when falling or being hit. In the future it will probably makes sense
            // to move it up to the Entity once other entities will have a need for it.
            {
                squashTime += gameTime.DeltaTime();

                if (Input.WasKeyPressed(Keys.L))
                    squashTime = 0f;

                // TODO: When chosing the curve values we need to make sure that the squashing stops
                // slightly before the peak of the jump has been reached. Currently jump height and
                // curve are synced to achive this, but maybe we can come up with an equation to
                // not have to tweak it by hand.
                var squashXCurve = new Curve();
                squashXCurve.Keys.Add(new CurveKey(0f, .5f));
                squashXCurve.Keys.Add(new CurveKey(.35f, 1f));

                var squashYCurve = new Curve();
                squashYCurve.Keys.Add(new CurveKey(0f, 1.25f));
                squashYCurve.Keys.Add(new CurveKey(.35f, 1f));

                var squashX = squashXCurve.Evaluate(squashTime);
                var squashY = squashYCurve.Evaluate(squashTime);

                // TODO: When squashing and stretching we probably want to change the origin,
                // so that the sprite bottom part (feet) does not go through the solids .
                animator.Scale = new Vector2(squashX, squashY);
            }

            // Movement.
            if (!hurting)
            {
                if (Input.IsKeyDown(Keys.Left))
                {
                    animator.Flip = SpriteEffects.FlipHorizontally;
                    DX += -speed;
                    Facing = -1;
                    state = HeroState.Running;
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    animator.Flip = SpriteEffects.None;
                    DX += speed;
                    Facing = 1;
                    state = HeroState.Running;
                }
                if (Input.WasKeyPressed(Keys.Up) && onGround)
                {
                    DY = jump;
                    state = HeroState.Jumping;
                    SoundFX.HeroJump.Play(volume: .7f, 0f, 0f);
                    jumpParticles.AddParticles(Position + new Vector2(0f, Level.CellSize / 2f), new Vector2(DX, DY) * 10);
                    squashTime = 0f;
                }
            }

            if (MathF.Abs(DX) < .01f)
                state = HeroState.Idle;
            else
                state = HeroState.Running;

            if (state == HeroState.Idle)
            {
                animator?.Play("Hero_Idle");
            }
            else if (state == HeroState.Running)
            {
                animator?.Play("Hero_Run");
                var feet = Position + new Vector2(0f, Level.CellSize / 2f);

                if (trailParticleTime >= TrailParticleInterval)
                {
                    trailParticleTime = 0f;
                    //runTrailParticles.AddParticles(feet, new Vector2(DX, DY));
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

            var wasOnGround = onGround;
            onGround = Level.IsWithinBounds(CX, CY) && Level.HasCollision(CX, CY + 1);

            // Landing.
            if (!wasOnGround && onGround)
            {
                // TODO: Need to come up with a way to spawn particles only sideways left and right without
                // them traveling upwards.
                //landingParticles.AddParticles(Position + new Vector2(0f, Level.CellSize / 2f), Vector2.Zero);
            }

            if (Input.IsKeyDown(Keys.Space))
                CurrentWeapon.PullTrigger();

            CurrentWeapon.Update(gameTime);
        }
    }
}
