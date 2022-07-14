namespace Gravity
{
    public class Weapons
    {
        public Weapon Pistol { get; init; }
        public Weapon Shotgun { get; init; }
        public Weapon Bazooka { get; init; }

        public Weapons(GameplayScreen gameplayScreen, Hero hero)
        {
            Pistol = new Pistol(gameplayScreen, hero);
            Shotgun = new Shotgun(gameplayScreen, hero);
            Bazooka = new Bazooka(gameplayScreen, hero);
        }

        public Weapon GetRandomWeapon(Weapon currentWeapon)
        {
            var newWeapon = Numerics.PickOne(Pistol, Shotgun, Bazooka);
            if (newWeapon == currentWeapon)
                return GetRandomWeapon(currentWeapon);
            return newWeapon;
        }
    }
}
