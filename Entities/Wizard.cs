using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class Wizard : Enemy
    {
        private IState<Wizard> currentState;

        public Wizard(GameplayScreen gameplayScreen) 
            : base(gameplayScreen, health: 200)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Wizard"));

            var anim = spriteSheet.CreateAnimation("Default", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Midground;
            sprite.Play(animID);

            LevelCollisions = false;
            Gravity = 0f;

            currentState = new WizardChaseState(gameplayScreen.Hero);
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
                ChangeState(new WizardDeathState());

            currentState.Execute(this);
        }

        public void ChangeState(IState<Wizard> newState)
        {
            currentState.Exit(this);
            currentState = newState;
            currentState.Enter(this);
        }
    }
}
