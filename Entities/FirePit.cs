using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class FirePit : Entity
    {
        public FirePit(GameplayScreen gameplayScreen, Vector2 position) : base(gameplayScreen)
        {
            Position = position;
            Gravity = 0f;
            Radius = 16;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Fire"));

            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 4 * 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(animID);

            Category = Mask.FirePit;
        }

        public override void OnEntityCollisionExit(Entity other)
        {
            if (other is Enemy enemy && !(enemy is Ghost) && enemy.IsAlive)
            {
                other.Destroy();
                var wizard = new Ghost(GameplayScreen) { Position = Position };
                GameplayScreen.AddEntity(wizard);
            }
            else if (other is Hero)
            {
                GameplayScreen.ExitScreen();
                GameplayScreen.ScreenManager.AddScreen(new MainMenuScreen());
            }
        }
    }
}
