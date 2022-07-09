using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Shotgun : Weapon
    {
        private readonly Sprite muzzleSprite;
        private readonly Game game;
        private readonly Hero hero;

        private const double ShotsPerSecond = 1.3;
        private const float ProjectileSpeed = 1f;

        private double muzzleTimer = 0;

        public Shotgun(Game game, Hero hero) : base(fireRate: 1.3f)
        {
            this.game = game;
            this.hero = hero;

            muzzleSprite = new Sprite(Textures.MuzzleFlash) { LayerDepth = .1f };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            muzzleTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        // TODO: Muzzle should be a separate entity we spawn on shot.
        public override void Draw(SpriteBatch batch)
        {
            if (muzzleTimer >= .0)
            {
                var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
                muzzleSprite.Position = position;
                muzzleSprite.Draw(batch);
            }
        }

        public override void OnShoot()
        {
            muzzleTimer = .0125;

            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing * ProjectileSpeed, -.35f);
            var cluster = new Cluster(game, position, velocity, damage: 100);
            game.AddEntity(cluster);

            SoundFX.ShotgunShot.Play(volume: .7f, 0f, 0f);
            game.WorldCamera.Shake(trauma: .465f);
        }
    }
}
