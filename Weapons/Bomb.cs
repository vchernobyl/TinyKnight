using TinyKnight.Coroutines;
using TinyKnight.Entities;
using TinyKnight.Graphics;
using TinyKnight.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace TinyKnight.Weapons
{
    public class BombProjectile : Entity
    {
        public BombProjectile(GameplayScreen gameplayScreen)
            : base(gameplayScreen, updateOrder: 200)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var tickingAnim = spriteSheet.CreateAnimation("Ticking", out int tickingAnimID);
            tickingAnim.AddFrame(new Rectangle(0, 24, 8, 8), duration: .25f);
            tickingAnim.AddFrame(new Rectangle(8, 24, 8, 8), duration: .25f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(tickingAnimID);
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy)
            {
                var explosion = new Explosion(GameplayScreen) { Position = Position };
                GameplayScreen.AddEntity(explosion);
                Destroy();
            }
        }
    }

    public class Bomb : Weapon
    {
        public Bomb(Hero hero, GameplayScreen gameplayScreen) :
            base(hero, gameplayScreen, fireRate: 1.5f, nameof(Bomb), updateOrder: 200)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;

            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var defaultAnim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            defaultAnim.AddFrame(new Rectangle(0, 24, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);
            
            Gravity = 0f;

            UpdatePosition();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void UpdatePosition()
        {
            var offset = new Vector2(hero.Facing * 5f, -1f);
            Position = hero.Position + offset;
        }

        protected override void Shoot()
        {
            var bomb = new BombProjectile(GameplayScreen) { Position = Position };
            GameplayScreen.AddEntity(bomb);
        }
    }
}
