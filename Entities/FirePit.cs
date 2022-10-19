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
            LevelCollisions = false;
            Gravity = 0f;
            Radius = 16;

            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Fire"));

            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 4 * 8, 8), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Foreground;
            sprite.Play(animID);
        }

        public override void OnEntityCollisionExit(Entity other)
        {
            if (other is Enemy && !(other is Wizard))
            {
                other.Destroy();
                var wizard = new Wizard(GameplayScreen) { Position = Position };
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
