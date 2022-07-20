using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Pistol : Weapon
    {
        private readonly GameplayScreen gamplayScreen;
        private readonly Hero hero;
        private readonly MuzzleFlash muzzleFlash;

        private const float Knockback = .025f;
        private const float Spread = 0.025f;
        private const float ProjectileSpeed = .95f;
        private const int Damage = 40;

        public Pistol(GameplayScreen gameplayScreen, Hero hero)
            : base(gameplayScreen, fireRate: 8f, name: "Pistol")
        {
            this.gamplayScreen = gameplayScreen;
            this.hero = hero;
            this.muzzleFlash = new MuzzleFlash(gameplayScreen) { Enabled = false };
            this.gamplayScreen.AddEntity(muzzleFlash);
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
            var velocity = new Vector2(hero.Facing * ProjectileSpeed, Random.FloatRange(-Spread, Spread));
            gamplayScreen.AddEntity(new Bullet(gamplayScreen) { Position = position, Velocity = velocity, Damage = Damage});
            muzzleFlash.Enabled = true;
            hero.Knockback(Knockback);
            SoundFX.PistolShot.Play();
        }
    }
}
