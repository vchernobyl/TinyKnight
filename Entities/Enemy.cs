using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity.Entities
{
    // TODO: Refactor. Every enemy should have an animation and death effects.
    public abstract class Enemy : Entity
    {
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        private readonly SoundEffect hitSound;

        public Action<Enemy> OnEnemyKilled;

        private int facing = Numerics.PickOne(1, -1);

        public float MaxSpeed { get; set; } = .1f;
        public float Acceleration { get; set; } = .01f;

        private const float Knockback = .05f;

        public Enemy(GameplayScreen gameplayScreen, int health, int updateOrder = 0)
            : base(gameplayScreen, updateOrder)
        {
            Health = health;

            Category = Mask.Enemy;

            var content = gameplayScreen.ScreenManager.Game.Content;
            hitSound = content.Load<SoundEffect>("SoundFX/Enemy_Hit");
        }

        public override void Update(GameTime gameTime)
        {
            if (Level.HasCollision(CX + 1, CY) && XR >= .7f ||
                Level.HasCollision(CX - 1, CY) && XR <= .3f)
            {
                facing = -facing;
            }

            if (MathF.Abs(DX) < MaxSpeed)
                DX += Acceleration * facing;

            Sprite.Flip = facing > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            base.Update(gameTime);
        }

        public void Heal(int amount)
        {
            Health += amount;
        }

        public void Kill()
        {
            Damage(Health);
        }

        public void Damage(int amount)
        {
            OnHit(amount);

            // Hit effects.
            {
                hitSound.Play(volume: .5f, 0f, 0f);
                Flash(duration: .125f, Color.White);

                DX -= facing * Knockback;

                // TODO: This currently causes a lot of problems when multiple enemies are being
                // hit at once. It looks like the sleep amount is accumulated and game freezes
                // for longer than expected.
                //Thread.Sleep(millisecondsTimeout: 20);

                TinyKnightGame.WorldCamera.Shake(trauma: .48f);
            }

            Health -= amount;
            if (Health <= 0)
            {
                OnDie();
                OnEnemyKilled?.Invoke(this);
                Destroy();
            }
        }

        public virtual void OnHit(int amount) { }

        public virtual void OnDie() { }
    }
}
