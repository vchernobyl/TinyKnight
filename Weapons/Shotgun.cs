using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Shotgun : Weapon
    {
        private readonly GameplayScreen gameplayScreen;

        public Shotgun(GameplayScreen gameplayScreen, Hero hero)
            : base(hero, fireRate: 1.3f, name: "Shotgun")
        {
            this.gameplayScreen = gameplayScreen;
        }

        public override void Shoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing, -.35f);
            var cluster = new Cluster(gameplayScreen, position, velocity, damage: 100);
            gameplayScreen.AddEntity(cluster);
            GravityGame.WorldCamera.Shake(trauma: .465f);
            SoundFX.ShotgunShot.Play();
        }
    }
}
