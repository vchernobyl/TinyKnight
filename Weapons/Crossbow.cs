﻿using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Crossbow : Weapon
    {
        private readonly GameplayScreen gamplayScreen;
        private readonly MuzzleFlash muzzleFlash;

        private const float Knockback = .025f;
        private const float Spread = 0.025f;
        private const float ProjectileSpeed = .95f;
        private const int Damage = 40;

        public Crossbow(GameplayScreen gameplayScreen, Hero hero)
            : base(hero, fireRate: 8f, name: nameof(Crossbow))
        {
            gamplayScreen = gameplayScreen;
            muzzleFlash = new MuzzleFlash(gameplayScreen) { Enabled = false };
            gamplayScreen.AddEntity(muzzleFlash);
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
            var velocity = new Vector2(hero.Facing * ProjectileSpeed, Random.FloatRange(-Spread, Spread));
            gamplayScreen.AddEntity(new Arrow(gamplayScreen) { Position = position, Velocity = velocity, Damage = Damage });
            muzzleFlash.Enabled = true;
            hero.Knockback(Knockback);
            SoundFX.PistolShot.Play();
        }
    }
}