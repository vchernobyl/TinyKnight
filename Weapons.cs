namespace Gravity
{
    public class Weapons
    {
        public Weapon Pistol { get; init; }
        public Weapon Shotgun { get; init; }
        public Weapon Bazooka { get; init; }

        public Weapons(Game game, Hero hero)
        {
            Pistol = new Pistol(game, hero);
            Shotgun = new Shotgun(game, hero);
            Bazooka = new Bazooka(game, hero);
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
