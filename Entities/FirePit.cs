using Gravity.Graphics;
using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class FirePit : Entity
    {
        private readonly ParticleSystem fireSystem;
        private readonly ParticleEmitter fireEmitter;

        public FirePit(GameplayScreen gameplayScreen, Vector2 position) : base(gameplayScreen)
        {
            Position = position;
            Gravity = 0f;
            Radius = 16;

            var game = gameplayScreen.ScreenManager.Game;
            fireSystem = new ParticleSystem(game, "Particles/FirePit");
            fireEmitter = new ParticleEmitter(fireSystem, particlesPerSecond: 80, position);
            game.Components.Add(fireSystem);

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

        public override void Update(GameTime gameTime)
        {
            fireEmitter.Update(gameTime, Position);
        }
    }
}
