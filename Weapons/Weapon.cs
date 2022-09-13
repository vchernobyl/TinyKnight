using Microsoft.Xna.Framework;

namespace Gravity
{
    public abstract class Weapon : Entity
    {
        public string Name { get; private set; }

        protected readonly Hero hero;

        private readonly float fireRate;
        private float fireTime;

        public Weapon(Hero hero, GameplayScreen gameplayScreen,
            float fireRate, string name) : base(gameplayScreen)
        {
            this.hero = hero;
            this.fireRate = fireRate;
            this.fireTime = 1f / fireRate;
            this.Name = name;
        }

        public override void Update(GameTime gameTime)
        {
            fireTime += gameTime.DeltaTime();
        }

        public void PullTrigger()
        {
            if (fireTime >= 1f / fireRate)
            {
                fireTime = 0f;
                Shoot();
            }
        }

        public virtual void Shoot() { }

        public abstract void UpdatePosition();
    }
}
