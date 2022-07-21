using Microsoft.Xna.Framework;

namespace Gravity
{
    // TODO: Once we get the weapon sprites, we will make this an Entity.
    // Weapon entity will be drawn on top of the player with some wobble effect,
    // and follow the hero movement.
    // This will also allow us to implement visual weapon kick-back.
    public abstract class Weapon
    {
        protected readonly Hero hero;

        private readonly GameplayScreen gameplayScreen;
        private readonly float fireRate;
        private readonly string name;
        private double fireTime;

        public Weapon(GameplayScreen gameplayScreen, Hero hero,
            float fireRate, string name)
        {
            this.gameplayScreen = gameplayScreen;
            this.hero = hero;
            this.fireRate = fireRate;
            fireTime = 1f / fireRate;
            this.name = name;
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

        public void Pickup()
        {
            var game = gameplayScreen.ScreenManager.Game;
            var weaponPickupText = new WeaponPickupText(game, name, hero.Position);
            game.Components.Add(weaponPickupText);
        }
    }
}
