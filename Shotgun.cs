using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Shotgun : Weapon
    {
        private readonly Game game;
        private readonly Hero hero;
        private readonly MuzzleFlash muzzleFlash;

        public Shotgun(Game game, Hero hero) : base(fireRate: 1.3f)
        {
            this.game = game;
            this.hero = hero;
            this.muzzleFlash = new MuzzleFlash(game) { Enabled = false };
            this.game.AddEntity(muzzleFlash);
        }

        public override void Update(GameTime gameTime)
        {
            if (muzzleFlash.Enabled)
                muzzleFlash.Position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;

            base.Update(gameTime);
        }

        public override void OnShoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing, -.35f);
            var cluster = new Cluster(game, position, velocity, damage: 100);
            game.AddEntity(cluster);
            game.WorldCamera.Shake(trauma: .465f);
            muzzleFlash.Enabled = true;
            SoundFX.ShotgunShot.Play();
        }
    }
}
