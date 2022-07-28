using Microsoft.Xna.Framework;

namespace Gravity
{
    // TODO: Once we get the weapon sprites, we will make this an Entity.
    // Weapon entity will be drawn on top of the player with some wobble effect,
    // and follow the hero movement.
    // This will also allow us to implement visual weapon kick-back.
    public abstract class Weapon
    {
        public string Name { get; private set; }

        protected readonly Hero hero;

        private readonly float fireRate;
        private double fireTime;

        public Weapon(Hero hero, float fireRate, string name)
        {
            this.hero = hero;
            this.fireRate = fireRate;
            fireTime = 1f / fireRate;
            this.Name = name;
        }

        public virtual void Update(GameTime gameTime)
        {
            fireTime += gameTime.DeltaTime();
        }

        public void PullTrigger()
        {
            if (fireTime >= 1f / fireRate)
            {
                fireTime = .0;
                Shoot();
            }
        }

        public virtual void Shoot() { }
    }
}
