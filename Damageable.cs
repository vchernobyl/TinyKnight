using System;
using System.Threading;

namespace Gravity
{
    // TODO: Should this also be applicable to the hero?
    public abstract class Damageable : Entity
    {
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public Action<Damageable>? OnDie;

        public Damageable(GameplayScreen gameplayScreen, Sprite sprite, int health)
            : base(gameplayScreen, sprite)
        {
            Health = health;
        }

        public void Heal(int amount)
        {
            Health += amount;
        }

        public void ReceiveDamage(int amount)
        {
            Health -= amount;
            SoundFX.EnemyHit.Play(volume: .5f, 0f, 0f);
            Flash(duration: .1f);
            Thread.Sleep(millisecondsTimeout: 20);

            // TODO: Move this to the projectile/weapon itself.
            gameplayScreen.WorldCamera.Shake(trauma: .48f);

            if (Health <= 0)
            {
                OnDie?.Invoke(this);
                Die();
            }
        }

        public abstract void Die();
    }
}
