﻿using TinyKnight.Graphics;
using TinyKnight.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TinyKnight.Entities
{
    public class Chest : Entity
    {
        private readonly List<Weapon> weapons;
        private readonly SoundEffect pickupSound;

        public Chest(GameplayScreen gameplayScreen) : base(gameplayScreen, updateOrder: 200)
        {
            Category = Mask.Item;

            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Chest"));
            var anim = spriteSheet.CreateAnimation("Zombie_Walk", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(animID);

            var hero = gameplayScreen.Hero;
            weapons = new List<Weapon>
            {
                new Crossbow(hero),
                new Axe(hero, gameplayScreen),
                new Cannon(hero, gameplayScreen),
                new Bomb(hero, gameplayScreen),
                new ThrowingStar(hero),
                new Shield(hero),
                new FireStaff(hero),
            };

            pickupSound = content.Load<SoundEffect>("SoundFX/Chest_Pickup");
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Hero hero)
            {
                var weapon = GetRandomWeapon();
                while (weapon.Name == hero.Weapon?.Name)
                    weapon = GetRandomWeapon();

                weapon.Position = hero.Position;
                hero.EquipWeapon(weapon);
                hero.UpdateScore();

                var game = GameplayScreen.ScreenManager.Game;
                var text = new WeaponPickupText(game, weapon.Name, Position);
                game.Components.Add(text);

                pickupSound.Play();

                Destroy();
            }
        }

        private Weapon GetRandomWeapon()
        {
            return weapons[Random.IntRange(0, weapons.Count)];
        }
    }
}
