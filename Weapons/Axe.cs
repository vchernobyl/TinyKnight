using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Gravity.Weapons
{
    public class AxeProjectile : Entity
    {
        private readonly int facing;

        public AxeProjectile(GameplayScreen gameplayScreen, int facing)
            : base(gameplayScreen)
        {
            this.facing = facing;
            Gravity = 0f;

            // TODO: We want the axe to collide only with enemies, but not with solids.
            // For this we need some collision mask functionality.
            //Debug.Assert(false);
            Collisions = false;

            var content = gameplayScreen.ScreenManager.Game.Content;
            //sprite = new Sprite(content.Load<Texture2D>("Textures/Axe"))
            //{
            //    Flip = facing > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally
            //};
        }

        public override void Update(GameTime gameTime)
        {
            DX = .75f * facing;
            //sprite.Rotation += 15f * facing * gameTime.DeltaTime();
        }
    }

    public class Axe : Weapon
    {
        public Axe(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 1f, nameof(Axe))
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            //sprite = new Sprite(content.Load<Texture2D>("Textures/Axe"));
        }

        public override void UpdatePosition()
        {
            if (hero.Facing > 0)
            {
                Position = hero.Position + new Vector2(5f, 0f);
                //sprite.Flip = SpriteEffects.None;
            }
            else
            {
                Position = hero.Position + new Vector2(-5f, 0f);
                //sprite.Flip = SpriteEffects.FlipHorizontally;
            }
        }

        public override void Shoot()
        {
            var p = new AxeProjectile(gameplayScreen, hero.Facing)
            {
                Position = Position
            };

            gameplayScreen.AddEntity(p);
        }
    }
}
