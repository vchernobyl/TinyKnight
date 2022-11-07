using Microsoft.Xna.Framework;

namespace Gravity.Weapons
{
    public abstract class Weapon : Entity
    {
        public string Name { get; private set; }

        protected readonly Hero hero;

        private readonly float fireRate;
        private float fireTime;

        public Weapon(Hero hero, GameplayScreen gameplayScreen,
            float fireRate, string name, int updateOrder = 0) : base(gameplayScreen, updateOrder)
        {
            this.hero = hero;
            this.fireRate = fireRate;
            fireTime = 1f / fireRate;
            Name = name;

            Category = Mask.Item;
            Collisions = Mask.None;
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

        protected virtual void Shoot() { }

        protected abstract void UpdatePosition();
    }
}
