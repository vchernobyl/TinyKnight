using Gravity.Entities;
using Gravity.GFX;
using Gravity.Particles;
using Gravity.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;
        private HeroState state = HeroState.Idle;

        private readonly ParticleSystem jumpParticles;

        public bool IsAlive { get { return Health > 0; } }

        private readonly int heroRunAnimID;
        private readonly int heroIdleAnimID;

        public Hero(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            CurrentWeapon = new Axe(this, gameplayScreen);
            //gameplayScreen.AddEntity(CurrentWeapon);

            Health = 3;
            Facing = 1;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;

            var texture = content.Load<Texture2D>("Textures/Tiny_Knight");
            var spriteSheet = new SpriteSheet(game.GraphicsDevice, texture);
            
            var heroRunAnim = spriteSheet.CreateAnimation("Hero_Run", out heroRunAnimID);
            heroRunAnim.AddFrame(new Rectangle(0, 0, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(8, 0, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(16, 0, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(24, 0, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(0, 8, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(8, 8, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(16, 8, 8, 8), .1f);
            heroRunAnim.AddFrame(new Rectangle(24, 8, 8, 8), .1f);

            var heroIdleAnim = spriteSheet.CreateAnimation("Hero_Idle", out heroIdleAnimID);
            heroIdleAnim.AddFrame(new Rectangle(0, 16, 8, 8), .1f);
            heroIdleAnim.AddFrame(new Rectangle(8, 16, 8, 8), .1f);
            heroIdleAnim.AddFrame(new Rectangle(16, 16, 8, 8), .1f);

            sprite = spriteSheet.Create();
            sprite.Play(heroIdleAnimID);
            sprite.LayerDepth = 1f;

            jumpParticles = new ParticleSystem(game, "Particles/HeroJumpParticleSettings");
            game.Components.Add(jumpParticles);
        }

        public override void OnEntityCollision(Entity other)
        {
            // TODO: Player death state.
            if (other is Enemy enemy && enemy.IsAlive && !hurting)
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

            hurtTime = Math.Max(0, hurtTime - gameTime.ElapsedGameTime.TotalSeconds);
            if (hurtTime == 0)
                hurting = false;

            // Movement.
            if (!hurting)
            {
                if (Input.IsKeyDown(Keys.Left))
                {
                    sprite.Flip = SpriteEffects.FlipHorizontally;
                    DX += -speed;
                    Facing = -1;
                    state = HeroState.Running;
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    sprite.Flip = SpriteEffects.None;
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

                    //animator.Scale = new Vector2(.4f, 1.35f);
                }
            }

            if (Input.WasKeyPressed(Keys.L))
                SquashX(.5f);

            if (Input.WasKeyPressed(Keys.K))
            {
                SquashY(.5f);
            }

            //animator.Scale = Numerics.Approach(animator.Scale, Vector2.One, gameTime.DeltaTime() * 2f);
            //animator.Origin = Numerics.Approach(animator.Origin, new Vector2(4f, 4f), gameTime.DeltaTime() * 20f);

            if (MathF.Abs(DX) < .01f)
                state = HeroState.Idle;
            else
                state = HeroState.Running;

            if (state == HeroState.Idle)
            {
                //animator.Play("Hero_Idle");
            }
            else if (state == HeroState.Running)
            {
                //animator.Play("Hero_Run");
            }

            if (MathF.Abs(DX) > 0.005f)
                sprite.Play(heroRunAnimID);
            else
                sprite.Play(heroIdleAnimID);

            var wasOnGround = onGround;
            onGround = Level.IsWithinBounds(CX, CY) && Level.HasCollision(CX, CY + 1);

            // Landing.
            if (!wasOnGround && onGround)
                SquashY(.5f);

            if (Input.IsKeyDown(Keys.Space))
                CurrentWeapon.PullTrigger();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            CurrentWeapon.UpdatePosition();
        }

        // TODO: These currently assume that every sprite/animator "normal"
        // scale is 1, which is not always the case. We need to multiply the
        // squash vector with the original scale.
        public void SquashX(float squash)
        {
            //animator.Scale = new Vector2(squash, 2f - squash);
        }

        public void SquashY(float squash)
        {
            //animator.Scale = new Vector2(2f - squash, squash);
            //animator.Origin.Y = 0f;
        }
    }
}
