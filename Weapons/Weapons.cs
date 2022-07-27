namespace Gravity
{
    public class Weapons
    {
        public Weapon Pistol { get; set; }
        public Weapon Shotgun { get; set; }
        public Weapon Bazooka { get; set; }
        public Weapon Railgun { get; set; }

        public Weapons(GameplayScreen gameplayScreen, Hero hero)
        {
            Pistol = new Pistol(gameplayScreen, hero);
            Shotgun = new Shotgun(gameplayScreen, hero);
            Bazooka = new Bazooka(gameplayScreen, hero);
            Railgun = new Railgun(gameplayScreen, hero);
        }

        public Weapon GetRandomWeapon(Weapon currentWeapon)
        {
            var newWeapon = Numerics.PickOne(Pistol, Shotgun, Bazooka);
            if (newWeapon == currentWeapon)
                return GetRandomWeapon(currentWeapon);

            newWeapon.Pickup();
            return newWeapon;
        }
    }
}
