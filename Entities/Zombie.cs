using Gravity.AI;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class Zombie : Enemy
    {
        public Zombie(GameplayScreen gameplayScreen)
            : base(gameplayScreen, health: 100, updateOrder: 200)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Zombie"));
            var walkAnim = spriteSheet.CreateAnimation("Zombie_Walk", out int walkAnimID);
            walkAnim.AddFrame(new Rectangle(0 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(1 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(2 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(3 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(4 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(5 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(6 * 8, 0, 8, 8), .1f);
            walkAnim.AddFrame(new Rectangle(7 * 8, 0, 8, 8), .1f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Midground;
            Sprite.Play(walkAnimID);

            Behaviour = new AIBehaviour(new WalkCommand() { Speed = 0.05f });
        }
    }
}
