using Gravity.Coroutines;
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

        private readonly CoroutineRunner coroutine;

        public Axe(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 1f, nameof(Axe))
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8, 8, 8), 0f);

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
            sprite.LayerDepth = 1f;

            coroutine = game.Services.GetService<CoroutineRunner>();

            Gravity = 0f;
            LevelCollisions = false;
            EntityCollisions = true;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (thrown && other is Enemy enemy)
                enemy.Damage(10);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // Only make axe follow the player if it's not flying around at this moment.
            if (thrown)
            {
                sprite.Rotation += .25f * hero.Facing;
                return;
            }

            if (hero.Facing > 0 && !thrown)
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
                coroutine.Run(Throw());
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
                var target = new Vector2(hero.Position.X + 5f * hero.Facing, hero.Position.Y);
                var dir = target - Position;
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
