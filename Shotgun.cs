using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Shotgun
    {
        // TODO: [Pellet] and [Bullet] classes are exactly the same. We can refactor it
        // later into a [Projectile] class.
        public class Pellet : Entity
        {
            public Vector2 Velocity;
            public readonly int Damage;

            private uint collisions = 0;

            public Pellet(Game game, Vector2 position, Vector2 velocity, int damage)
                : base(game, new Sprite(Textures.Bullet))
            {
                Position = position;
                Velocity = velocity;
                Damage = damage;
                Collision = true;
            }

            public override void OnLevelCollision(Vector2 normal)
            {
                Velocity = Vector2.Reflect(Velocity, normal);

                if (collisions++ == 10)
                    IsActive = false;
            }

            public override void Update(GameTime gameTime)
            {
                DX = Velocity.X;
                DY = Velocity.Y;
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

                for (int i = 0; i < 6; i++)
                {
                    var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
                    var velocity = new Vector2(hero.Facing * ProjectileSpeed, Random.FloatRange(-.75f, .75f));
                    var bullet = new Pellet(game, position, velocity, Damage);
                    game.AddEntity(bullet);
                    hero.Knockback(Knockback);
                }

                SoundFX.ShotgunShot.Play(volume: .7f, 0f, 0f);
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
