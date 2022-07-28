namespace Gravity
{
    public class Box : Entity
    {
        private readonly Weapons weapons;

        public Box(GameplayScreen gameplayScreen)
            : base(gameplayScreen, new Sprite(Textures.Box))
        {
            weapons = new Weapons(gameplayScreen, gameplayScreen.Hero);
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Hero hero)
            {
                var newWeapon = weapons.GetRandomWeapon(hero.CurrentWeapon);
                hero.CurrentWeapon = newWeapon;

                var game = gameplayScreen.ScreenManager.Game;
                var weaponPickupText = new WeaponPickupText(game, newWeapon.Name, hero.Position);
                game.Components.Add(weaponPickupText);

                IsActive = false;
            }
        }
    }
}
