using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gravity
{
    public class Shotgun
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

        public class Pellet : Entity, IProjectile
        {
            public Vector2 Velocity { get; set; }
            public int Damage { get; set; }

            private uint collisions = 0;

            public Pellet(Game game, Vector2 position, Vector2 velocity, int damage)
                : base(game, new Sprite(Textures.Bullet))
            {
                Position = position;
                Velocity = velocity;
                Damage = damage;
                Collision = true;
            }

            public override void OnEntityCollision(Entity other)
            {
                if (other is Enemy && other.Collision)
                {
                    IsActive = false;
                }
            }

            public override void OnLevelCollision(Vector2 normal)
            {
                Velocity = Vector2.Reflect(Velocity, normal);

                if (collisions++ == 4)
                    IsActive = false;
            }

            public override void Update(GameTime gameTime)
            {
                DX = Velocity.X;
                DY = Velocity.Y;

                sprite.Rotation = Numerics.VectorToRadians(new Vector2(DX, DY));

                base.Update(gameTime);
            }

            public override void Draw(SpriteBatch batch)
            {
                DX = Velocity.X;

                if (Velocity.X < 0)
                    sprite.Flip = SpriteEffects.FlipHorizontally;
                else
                    sprite.Flip = SpriteEffects.None;

                base.Draw(batch);
            }
        }

        private readonly Sprite muzzleSprite;
        private readonly Game game;
        private readonly Hero hero;

        private const double ShotsPerSecond = 1.3;
        private const float Knockback = .15f;
        private const float Spread = .125f;
        private const float ProjectileSpeed = 1f;
        private const int Damage = 35;
        private const int PelletAmount = 7;

        private double shotTimer = 1 / ShotsPerSecond;
        private double muzzleTimer = 0;

        public Shotgun(Game game, Hero hero)
        {
            this.game = game;
            this.hero = hero;

            muzzleSprite = new Sprite(Textures.MuzzleFlash) { LayerDepth = .1f };
        }

        public void Update(GameTime gameTime)
        {
            shotTimer += gameTime.ElapsedGameTime.TotalSeconds;
            muzzleTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (shotTimer >= 1 / ShotsPerSecond && Input.IsKeyDown(Keys.Space))
            {
                shotTimer = 0;
                muzzleTimer = .0125;

                var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
                var velocity = new Vector2(hero.Facing * ProjectileSpeed, -.35f);
                var cluster = new Cluster(game, position, velocity, damage: 100);
                game.AddEntity(cluster);

                SoundFX.ShotgunShot.Play(volume: .7f, 0f, 0f);
                game.WorldCamera.Shake(trauma: .465f);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (muzzleTimer >= .0)
            {
                var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
                muzzleSprite.Position = position;
                muzzleSprite.Draw(batch);
            }
        }
    }
}
