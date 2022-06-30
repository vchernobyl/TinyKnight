﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Cluster : Entity, IProjectile
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }

        private readonly Sprite muzzleSprite;
        private readonly Timer deathTimer;

        private double muzzleTime = 0;
        private bool collided = false;

        public Cluster(Game game, Vector2 position, Vector2 velocity, int damage)
            : base(game, new Sprite(Textures.Bullet))
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Collision = true;
            DY = velocity.Y;
            DX = velocity.X;
            FrictionX = .96f;

            muzzleSprite = new Sprite(Textures.MuzzleFlash) { LayerDepth = 0f };
            deathTimer = new Timer(duration: .05, onEnd: () => { IsActive = false; });
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Enemy enemy && enemy.Alive)
            {
                // TODO: Call enemy.Hurt() instead.
                enemy.OnEntityCollision(this);
                DischargeCluster(-Vector2.UnitY);
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            DischargeCluster(normal);
        }

        private void DischargeCluster(Vector2 normal)
        {
            if (!collided)
            {
                collided = true;
                muzzleTime = .05;
                deathTimer.Start();

                for (int i = 0; i < 6; i++)
                {
                    var radians = Numerics.VectorToRadians(normal) + Random.FloatRange(-MathF.PI / 4f, MathF.PI / 4f);
                    var velocity = Numerics.RadiansToVector(radians);
                    game.AddEntity(new Pellet(game, Position, velocity, damage: 90));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (collided)
            {
                DX = 0f;
                DY = 0f;
            }
            deathTimer.Update(gameTime);
            muzzleTime = Math.Max(0, muzzleTime - gameTime.ElapsedGameTime.TotalSeconds);
            sprite.Rotation = Numerics.VectorToRadians(new Vector2(DX, DY));
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            if (muzzleTime > 0)
            {
                muzzleSprite.Position = Position;
                muzzleSprite.Draw(batch);
            }

            base.Draw(batch);
        }
    }
}
