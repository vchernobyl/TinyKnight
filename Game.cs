using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch spriteBatch;
        private LevelManager levelManager;

        public Game()
        {
            new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            levelManager = new LevelManager(Services);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.WasKeyPressed(Keys.N))
                levelManager.NextLevel();

            if (Keyboard.WasKeyPressed(Keys.R))
                levelManager.SetLevel(0);

            levelManager.CurrentLevel.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            levelManager.CurrentLevel.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
