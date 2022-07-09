using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    // TODO: Once we get the weapon sprites, we will make this an Entity.
    // Weapon entity will be drawn on top of the player with some wobble effect,
    // and follow the hero movement.
    // This will also allow us to implement visual weapon kick-back.
    public abstract class Weapon
    {
        private readonly float fireRate;
        private double fireTime;

        public Weapon(float fireRate)
        {
            this.fireRate = fireRate;
            this.fireTime = 1f / fireRate;
        }

        // TODO: Won't be needed because input handling will be pulled out
        // into the hero.
        public virtual void Update(GameTime gameTime)
        {
            fireTime += gameTime.DeltaTime();
        }

        // TODO: Won't be needed because the muzzle will be a separate entity.
        public virtual void Draw(SpriteBatch batch) { }

        public void Shoot()
        {
            if (fireTime >= 1f / fireRate)
            {
                fireTime = .0;
                OnShoot();
            }
        }

        public virtual void OnShoot() { }
    }
}
