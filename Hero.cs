using Gravity.Entities;
using Gravity.Graphics;
using Gravity.Particles;
using Gravity.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    public class Hero : Entity
    {
        public int Facing { get; private set; }
        public int Health { get; private set; }
        public const int MaxHealth = 3;

        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;

        private readonly ParticleSystem jumpParticles;

        public bool IsAlive { get { return Health > 0; } }

        private readonly int heroRunAnimID;
        private readonly int heroIdleAnimID;

        private readonly InputAction moveLeft;
        private readonly InputAction moveRight;
        private readonly InputAction jump;
        private readonly InputAction shoot;

        private readonly SoundEffect jumpSound;
        private readonly SoundEffect hurtSound;

        public Weapon? Weapon { get; private set; }

        private int score;
        public event Action<int> OnScoreUpdate;

        public bool Invincible = false;

        public Hero(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            EquipWeapon(new Bomb(this, gameplayScreen));

            Health = 3;
            Facing = 1;

            Category = Mask.Player;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;

            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Tiny_Knight"));

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

            Sprite = spriteSheet.Create();
            Sprite.Play(heroIdleAnimID);
            Sprite.LayerDepth = DrawLayer.Midground;

            jumpSound = content.Load<SoundEffect>("SoundFX/Hero_Jump");
            hurtSound = content.Load<SoundEffect>("SoundFX/Hero_Hurt");

            jumpParticles = new ParticleSystem(game, "Particles/HeroJumpParticleSettings");
            game.Components.Add(jumpParticles);

            moveLeft = new InputAction(new[] { Buttons.DPadLeft }, new[] { Keys.Left }, false);
            moveRight = new InputAction(new[] { Buttons.DPadRight }, new[] { Keys.Right }, false);
            jump = new InputAction(new[] { Buttons.A }, new[] { Keys.Up }, true);
            shoot = new InputAction(new[] { Buttons.X }, new[] { Keys.Space }, false);
        }

        public void EquipWeapon(Weapon? weapon)
        {
            if (Weapon != null)
                GameplayScreen.RemoveEntity(Weapon);

            Weapon = weapon;

            if (weapon != null)
                GameplayScreen.AddEntity(weapon);
        }

        public void UpdateScore()
        {
            OnScoreUpdate?.Invoke(++score);
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy && enemy.IsAlive && !hurting)
            {
                if (Invincible)
                {
                    enemy.Kill();
                    return;
                }

                Health--;
                if (Health <= 0)
                {
                    GameplayScreen.ExitScreen();
                    GameplayScreen.ScreenManager.AddScreen(new MainMenuScreen());
                }

                hurting = true;
                hurtTime = .2;
                hurtSound.Play();

                Flash(duration: .15f, Color.Red);

                DY = -.3f;
                DX = Math.Sign(XX - other.XX) * .3f;
            }
        }

        public override void HandleInput(InputState input)
        {
            var speed = .0175f;

            // TODO: The problem with having a separate input handling method
            // is that it is triggered before Update method (as it should).
            // The problem is that this allows us to trigger some game logic
            // which will cause states between entities to be out of sync or
            // in general in weird states. For example, PullTrigger within
            // HandleInput method can spawn an entity, this entity will not
            // have the right Hero position, since Hero.Update is yet to be called.
            // This will make the spawned entity to have a hero position which is
            // lagging behind by one frame.
            // 
            // Possible solution: static InputManager class (https://github.com/SimonDarksideJ/XNAGameStudio/blob/archive/Samples/RolePlayingGame_4_0_Win_Xbox/RolePlayingGame_4_0_Win_Xbox/RolePlayingGame/InputManager.cs)
            // Or something similar.

            // Movement.
            if (!hurting)
            {
                if (moveLeft.Evaluate(input))
                {
                    Sprite.Flip = SpriteEffects.FlipHorizontally;
                    DX += -speed;
                    Facing = -1;
                }
                if (moveRight.Evaluate(input))
                {
                    Sprite.Flip = SpriteEffects.None;
                    DX += speed;
                    Facing = 1;
                }
                if (jump.Evaluate(input) && onGround)
                {
                    Jump();
                }
            }

            if (shoot.Evaluate(input))
                Weapon?.PullTrigger();
        }

        public override void Update(GameTime gameTime)
        {
            hurtTime = Math.Max(0, hurtTime - gameTime.ElapsedGameTime.TotalSeconds);
            if (hurtTime == 0)
                hurting = false;

            Sprite.Scale = Numerics.Approach(Sprite.Scale, Vector2.One, gameTime.DeltaTime() * 2f);
            Sprite.Origin = Numerics.Approach(Sprite.Origin, new Vector2(4f, 4f), gameTime.DeltaTime() * 20f);

            if (MathF.Abs(DX) > 0.005f)
                Sprite.Play(heroRunAnimID);
            else
                Sprite.Play(heroIdleAnimID);

            var wasOnGround = onGround;
            onGround = Level.IsWithinBounds(CX, CY) && Level.HasCollision(CX, CY + 1);

            // Landing.
            if (!wasOnGround && onGround)
                SquashY(.5f);
        }

        // TODO: These currently assume that every sprite/animator "normal"
        // scale is 1, which is not always the case. We need to multiply the
        // squash vector with the original scale.
        public void SquashX(float squash)
        {
            Sprite.Scale = new Vector2(squash, 2f - squash);
        }

        public void SquashY(float squash)
        {
            Sprite.Scale = new Vector2(2f - squash, squash);
            Sprite.Origin.Y = 0f;
        }

        public void Jump()
        {
            const float force = -1.25f;
            DY = force;

            jumpSound.Play();
            jumpParticles.AddParticles(Position + new Vector2(0f, Level.CellSize / 2f), new Vector2(DX, DY) * 10);
        }
    }
}
