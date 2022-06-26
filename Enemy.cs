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
        private double deathTimer = 2.0;
        private bool startDeathAnimation = false;

        public Enemy(Game game, Sprite sprite, Spawner spawner)
            : base(game, sprite)
        {
            this.spawner = spawner;
            hitSound = game.Content.Load<SoundEffect>("SoundFX/Enemy_Hit");
            facing = Numerics.PickOne(-1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (!startDeathAnimation && level.HasCollision(CX, CY + 1))
                DX = Math.Sign(facing) * .1f;

            if (!startDeathAnimation &&
                ((level.HasCollision(CX + 1, CY) && XR >= .7f) ||
                (level.HasCollision(CX - 1, CY) && XR <= .3f)))
            {
                facing = -facing;
                DX = Math.Sign(facing) * .1f;
            }

            if (facing > 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            if (!startDeathAnimation &&
                level.IsWithinBounds(CX, CY) &&
                level[CX, CY].Type == Cell.CellType.Water)
            {
                SetCoordinates(spawner.Position.X, spawner.Position.Y);
            }

            if (startDeathAnimation)
            {
                deathTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                sprite.Rotation += Random.FloatRange(
                    MathHelper.PiOver4,
                    MathHelper.PiOver2) * DX;

                if (deathTimer <= 0f)
                    IsActive = false;
            }

            base.Update(gameTime);
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Shotgun.Pellet bullet)
            {
                other.IsActive = false;
                Health -= bullet.Damage;
                hitSound.Play(.5f, 0f, 0f);

                Flash(duration: .1);

                // Knockback
                DX = Math.Sign(other.DX) * .085f;

                game.WorldCamera.Shake(.425f);
                Thread.Sleep(10);

                if (Health <= 0)
                {
                    DY = Random.FloatRange(-.4f, -.5f);
                    DX = Math.Sign(bullet.Velocity.X) * Random.FloatRange(.1f, .2f);
                    startDeathAnimation = true;
                    Collision = false;

                    var coin = new Coin(game) { Position = this.Position };
                    game.AddEntity(coin);
                }
            }
        }

        public override void OnDestroy()
        {
            game.Hero.EnemiesKilled++;
            OnDie?.Invoke(this);
        }
    }
}
