using Gravity.Entities;
using Gravity.GFX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Gravity.Weapons
{
    public class Axe : Weapon
    {
        private bool thrown = false;

        public Axe(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 1f, nameof(Axe))
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8, 8, 8), 0f);

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);

            Gravity = 0f;
            Collisions = true;
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Enemy enemy)
                enemy.Damage(50);
        }

        public override void UpdatePosition()
        {
            // Only make axe follow the player if it's not flying around at this moment.
            if (thrown)
            {
                sprite.Rotation += .25f * hero.Facing;
                return;
            }

            if (hero.Facing > 0)
            {
                Position = hero.Position + new Vector2(5f, 0f);
                sprite.Flip = SpriteEffects.None;
            }
            else
            {
                Position = hero.Position + new Vector2(-5f, 0f);
                sprite.Flip = SpriteEffects.FlipHorizontally;
            }
        }

        public override void Shoot()
        {
            if (!thrown)
                GravityGame.Runner.Run(Throw());
        }

        private IEnumerator Throw()
        {
            thrown = true;
            var frames = -1;
            var throwDirection = hero.Facing;
            while (++frames < 10)
            {
                DX = 1.2f * throwDirection;
                yield return null;
            }

            while (Vector2.Distance(Position, hero.Position) > 8f)
            {
                var dir = hero.Position - Position;
                dir.Normalize();

                DX += dir.X * .15f;
                DY += dir.Y * .15f;

                yield return null;
            }
            thrown = false;
            sprite.Rotation = 0f;
        }
    }
}
