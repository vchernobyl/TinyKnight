﻿using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Shotgun : Weapon
    {
        private readonly GameplayScreen gameplayScreen;
        private readonly MuzzleFlash muzzleFlash;

        public Shotgun(GameplayScreen gameplayScreen, Hero hero)
            : base(gameplayScreen, hero, fireRate: 1.3f, name: "Shotgun")
        {
            this.gameplayScreen = gameplayScreen;
            this.muzzleFlash = new MuzzleFlash(gameplayScreen) { Enabled = false };
            this.gameplayScreen.AddEntity(muzzleFlash);
        }

        public override void Update(GameTime gameTime)
        {
            if (muzzleFlash.Enabled)
                muzzleFlash.Position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;

            base.Update(gameTime);
        }

        public override void Shoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing, -.35f);
            var cluster = new Cluster(gameplayScreen, position, velocity, damage: 100);
            gameplayScreen.AddEntity(cluster);
            GravityGame.WorldCamera.Shake(trauma: .465f);
            muzzleFlash.Enabled = true;
            SoundFX.ShotgunShot.Play();
        }
    }
}