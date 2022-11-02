﻿using Gravity.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Threading;

namespace Gravity.Entities
{
    // TODO: Refactor. Every enemy should have an animation and death effects.
    public abstract class Enemy : Entity
    {
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public float RotationSpeed { get; set; }
        public AIBehaviour? Behaviour { get; set; }

        private readonly SoundEffect hitSound;

        public Enemy(GameplayScreen gameplayScreen, int health, int updateOrder = 0)
            : base(gameplayScreen, updateOrder)
        {
            Health = health;

            var content = gameplayScreen.ScreenManager.Game.Content;
            hitSound = content.Load<SoundEffect>("SoundFX/Enemy_Hit");
        }

        public override void Update(GameTime gameTime)
        {
            Sprite.Rotation += RotationSpeed;

            var command = Behaviour?.Update(gameTime);
            command?.Execute(this);

            base.Update(gameTime);
        }

        public void Heal(int amount)
        {
            Health += amount;
        }

        public void Damage(int amount)
        {
            OnHit(amount);

            // Hit effects.
            {
                hitSound.Play(volume: .5f, 0f, 0f);
                Flash(duration: .1f, Color.Red);
                Thread.Sleep(millisecondsTimeout: 20);
                GravityGame.WorldCamera.Shake(trauma: .48f);
            }

            Health -= amount;
            if (Health <= 0)
            {
                OnDie();
            }
        }

        public virtual void OnHit(int amount) { }

        public virtual void OnDie() { }
    }
}
