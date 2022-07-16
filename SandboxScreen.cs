using Microsoft.Xna.Framework;

namespace Gravity
{
    public class SandboxScreen : GameScreen
    {
        #region Particles
        private SmokePlumeParticleSystem? smokePlume;
        private ExplosionParticleSystem? explosion;
        private ExplosionSmokeParticleSystem? smoke;
        
        private const float TimeBetweenSmokePlumePuffs = .5f;
        private float timeTillPuff = 0f;

        private const float TimeBetweenExplosions = 2f;
        private float timeTillExplosion = 0f;
        #endregion

        private GraphicsDeviceManager? graphics;

        public override void LoadContent()
        {
            graphics = (GraphicsDeviceManager)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceManager));

            var game = (Game)ScreenManager.Game;

            smokePlume = new SmokePlumeParticleSystem(game, 2);
            game.Components.Add(smokePlume);

            explosion = new ExplosionParticleSystem(game, 1);
            game.Components.Add(explosion);

            smoke = new ExplosionSmokeParticleSystem(game, 9);
            game.Components.Add(smoke);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Smoke.
            timeTillPuff -= (float)gameTime.DeltaTime();
            if (timeTillPuff < 0f)
            {
                var where = Vector2.Zero;
                where.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                where.Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
                smokePlume?.AddParticles(where);
                timeTillPuff = TimeBetweenSmokePlumePuffs;
            }

            // Explosion.
            timeTillExplosion -= (float)gameTime.DeltaTime();
            if (timeTillExplosion < 0)
            {
                Vector2 where = Vector2.Zero;
                // create the explosion at some random point on the screen.
                where.X = Random.IntRange(0, graphics!.GraphicsDevice.Viewport.Width);
                where.Y = Random.IntRange(0, graphics!.GraphicsDevice.Viewport.Height);

                // the overall explosion effect is actually comprised of two particle
                // systems: the fiery bit, and the smoke behind it. add particles to
                // both of those systems.
                explosion?.AddParticles(where);
                smoke?.AddParticles(where);

                // reset the timer.
                timeTillExplosion = TimeBetweenExplosions;
            }
        }
    }
}
