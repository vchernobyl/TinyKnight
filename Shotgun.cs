using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Shotgun : IWeapon
    {
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
