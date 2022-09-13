using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Crossbow : Weapon
    {
        private const float Spread = 0.025f;
        private const float ProjectileSpeed = .95f;
        private const int Damage = 40;

        public Crossbow(GameplayScreen gameplayScreen, Hero hero)
            : base(hero, gameplayScreen, fireRate: 8f, name: nameof(Crossbow))
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            sprite = new Sprite(content.Load<Texture2D>("Textures/Crossbow"))
            {
                LayerDepth = .9f,
                Origin = Vector2.Zero,
            };

            Collision = false;
            Gravity = 0f;
        }

        public override void Shoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            var velocity = new Vector2(hero.Facing * ProjectileSpeed, Random.FloatRange(-Spread, Spread));
            gameplayScreen.AddEntity(new Arrow(gameplayScreen) { Position = position, Velocity = velocity, Damage = Damage });
            SoundFX.PistolShot.Play();
        }

        public override void UpdatePosition()
        {
            if (hero.Facing > 0)
            {
                Position = hero.Position + new Vector2(-1f, -2f);
                sprite.Flip = SpriteEffects.None;
            }
            else
            {
                var anchor = new Vector2(-7f, -2f);
                Position = hero.Position + anchor;
                sprite.Flip = SpriteEffects.FlipHorizontally;
            }
        }
    }
}
