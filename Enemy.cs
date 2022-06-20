using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;

namespace Gravity
{
    public class Enemy : Entity
    {
        public int Health { get; set; } = 100;

        public event Action<Enemy>? OnDie;

        private readonly Spawner spawner;
        private readonly SoundEffect hitSound;

        private int facing;
        private double deathTimer = .6;
        private bool startDeathAnimation = false;

        public Enemy(Game game, Sprite sprite, Level level, Spawner spawner)
            : base(game, sprite, level)
        {
            this.spawner = spawner;
            hitSound = game.Content.Load<SoundEffect>("SoundFX/Enemy_Hit");
            facing = Numerics.Pick(RNG.IntRange(0, 2), -1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (level.HasCollision(CX, CY + 1))
                DX = Math.Sign(facing) * .1f;

            if ((level.HasCollision(CX + 1, CY) && XR >= .7f) || (level.HasCollision(CX - 1, CY) && XR <= .3f))
            {
                facing = -facing;
                DX = Math.Sign(facing) * .1f;
            }

            if (facing > 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            if (level[CX, CY].Type == Cell.CellType.Water)
                SetCoordinates(spawner.Position.X, spawner.Position.Y);

            if (startDeathAnimation)
            {
                deathTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                sprite.Rotation += .1f;

                if (deathTimer <= 0f)
                    IsActive = false;
            }

            base.Update(gameTime);
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Bullet)
            {
                other.IsActive = false;
                Health -= Bullet.Damage;
                hitSound.Play(.5f, 0f, 0f);

                Flash(duration: .1);

                // Knockback
                DX = Math.Sign(other.DX) * .085f;

                game.Camera.Shake(.425f);
                Thread.Sleep(10);

                if (Health <= 0)
                {
                    DY += -.75f;
                    DX += -facing * .1f;
                    startDeathAnimation = true;
                }
            }
        }

        public override void OnDestroy()
        {
            game.Hud.EnemiesKilled++;
            OnDie?.Invoke(this);
        }
    }
}
