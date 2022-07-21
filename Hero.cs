﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    public class Hero : Entity
    {
        public uint EnemiesKilled { get; set; }
        public int Facing { get; private set; } = -1;

        private readonly Weapons weapons;
        private Weapon weapon;

        private bool onGround = false;
        private bool hurting = false;
        private double hurtTime = 0;

        public Hero(GameplayScreen gamplayScreen) : base(gamplayScreen, new Sprite(Textures.Hero))
        {
            weapons = new Weapons(gamplayScreen, this);
            weapon = weapons.Bazooka;

            foreach (var entity in gamplayScreen.Entities)
            {
                if (entity is Portal &&  entity is Damageable portal)
                    portal.OnDie += (portal) => { weapon = weapons.GetRandomWeapon(weapon); };
            }
        }

        public void Knockback(float amount)
        {
            DX += -Facing * amount;
        }

        public override void OnEntityCollision(Entity other)
        {
            // TODO: Player death state.
            if (other is Damageable enemy && enemy.IsAlive && !hurting)
            {
                hurting = true;
                hurtTime = .2;
                SoundFX.HeroHurt.Play();
                Flash(duration: .15f, Color.Red);

                DY = -.3f;
                DX = Math.Sign(XX - other.XX) * .3f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .0175f;
            var jump = -1f;

            hurtTime = Math.Max(0, hurtTime - gameTime.ElapsedGameTime.TotalSeconds);
            if (hurtTime == 0)
                hurting = false;

            // Movement.
            if (!hurting)
            {
                if (Input.IsKeyDown(Keys.Left))
                {
                    sprite.Flip = SpriteEffects.None;
                    DX += -speed;
                    Facing = -1;
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    sprite.Flip = SpriteEffects.FlipHorizontally;
                    DX += speed;
                    Facing = 1;
                }
                if (Input.WasKeyPressed(Keys.Up) && onGround)
                {
                    DY = jump;
                    SoundFX.HeroJump.Play(volume: .7f, 0f, 0f);
                }
            }

            // Weapon switching.
            if (Input.WasKeyPressed(Keys.D1))
            {
                weapon = weapons.Pistol;
                weapon.Pickup();
            }
            if (Input.WasKeyPressed(Keys.D2))
            {
                weapon = weapons.Shotgun;
                weapon.Pickup();
            }
            if (Input.WasKeyPressed(Keys.D3))
            {
                weapon = weapons.Bazooka;
                weapon.Pickup();
            }

            onGround = Level.HasCollision(CX, CY + 1);

            if (Input.IsKeyDown(Keys.Space))
                weapon.PullTrigger();

            weapon.Update(gameTime);
        }
    }
}
