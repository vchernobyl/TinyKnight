namespace Gravity
{
    public class Weapons
    {
        public IWeapon Pistol { get; init; }
        public IWeapon Shotgun { get; init; }

        public Weapons(Game game, Hero hero)
        {
            Pistol = new Pistol(game, hero);
            Shotgun = new Shotgun(game, hero);
        }

        public IWeapon GetRandomWeapon(IWeapon currentWeapon)
        {
            var newWeapon = Numerics.PickOne(Pistol, Shotgun);
            if (newWeapon == currentWeapon)
                return GetRandomWeapon(currentWeapon);
            return newWeapon;
        }
    }
}
