using System.Threading;

namespace Gravity.Entities
{
    public abstract class Enemy : Entity
    {
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public Enemy(GameplayScreen gameplayScreen, int health)
            : base(gameplayScreen)
        {
            Health = health;
        }

        public void Heal(int amount)
        {
            Health += amount;
        }

        public void Damage(int amount)
        {
            OnHit(amount);

            // Hit effects.
            {
                SoundFX.EnemyHit.Play(volume: .5f, 0f, 0f);
                Flash(duration: .1f);
                Thread.Sleep(millisecondsTimeout: 20);
                GravityGame.WorldCamera.Shake(trauma: .48f);
            }

            Health -= amount;
            if (Health <= 0)
                Die();
        }

        public virtual void OnHit(int amount) { }

        public abstract void Die();
    }
}
