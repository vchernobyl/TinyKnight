using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Pistol : Weapon
    {
        private readonly Sprite muzzleSprite;
        private readonly Game game;
        private readonly Hero hero;

        private const float Knockback = .025f;
        private const float Spread = 0.025f;
        private const float ProjectileSpeed = .95f;
        private const int Damage = 40;

        private double muzzleTimer = 0;

        public Pistol(Game game, Hero hero) : base(fireRate: 8f)
        {
            this.game = game;
            this.hero = hero;

            muzzleSprite = new Sprite(Textures.MuzzleFlash) { LayerDepth = .1f };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            muzzleTimer -= gameTime.DeltaTime();
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
            var velocity = new Vector2(hero.Facing * ProjectileSpeed, Random.FloatRange(-Spread, Spread));
            game.AddEntity(new Bullet(game) { Position = position, Velocity = velocity, Damage = Damage});
            hero.Knockback(Knockback);

            SoundFX.PistolShot.Play(volume: .7f, 0f, 0f);
        }
    }
}
