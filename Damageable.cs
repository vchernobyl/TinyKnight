using System;
using System.Threading;

namespace Gravity
{
    public abstract class Damageable : Entity
    {
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public Action<Damageable>? OnHit;
        public Action<Damageable>? OnDie;

        public Damageable(Game game, Sprite sprite, int health)
            : base(game, sprite)
        {
            Health = health;
        }

        public void ReceiveDamage(int amount)
        {
            Health -= amount;
            SoundFX.EnemyHit.Play();
            Flash(duration: .1f);
            Thread.Sleep(millisecondsTimeout: 10);

            OnHit?.Invoke(this);

            if (Health <= 0)
            {
                OnDie?.Invoke(this);
                Die();
            }
        }

        public abstract void Die();
    }
}
