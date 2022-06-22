using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Pistol
    {
        private readonly Sprite muzzleSprite;
        private readonly Game game;
        private readonly Hero hero;

        private const double ShotsPerSecond = 8;
        private const float Knockback = .05f;

        private double shotTimer = 1 / ShotsPerSecond;
        private double muzzleTimer = 0;

        public Pistol(Game game, Hero hero)
        {
            this.game = game;
            this.hero = hero;

            muzzleSprite = new Sprite(Textures.MuzzleFlash)
            {
                LayerDepth = .1f
            };
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
                var bullet = new Bullet(game, position, Vector2.UnitX * hero.Facing);
                game.AddEntity(bullet);
                hero.Knockback(Knockback);

                SoundFX.PistolShot.Play(volume: .7f, 0f, 0f);
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
