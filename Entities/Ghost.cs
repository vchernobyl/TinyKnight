using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class Ghost : Enemy
    {
        private readonly Hero hero;

        public Ghost(GameplayScreen gameplayScreen)
            : base(gameplayScreen, health: 200, updateOrder: 200)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Ghost"));

            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Midground;
            sprite.Play(animID);

            LevelCollisions = false;
            Gravity = 0f;

            hero = gameplayScreen.Hero;
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
                Destroy();

            const float speed = 0.05f;
            var dir = Vector2.Normalize(hero.Position - Position);
            DX = dir.X * speed;
            DY = dir.Y * speed;
        }
    }
}
