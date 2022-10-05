using Gravity.Coroutines;
using Gravity.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class GravityGame : Game
    {
        public static Camera WorldCamera { get; private set; }
        public static Camera UiCamera { get; private set; }

        private readonly GraphicsDeviceManager graphics;
        private readonly ScreenManager screenManager;

        // TODO: Maybe make it a service?
        public static readonly CoroutineRunner Runner = new CoroutineRunner();

        public GravityGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
            Services.AddService(screenManager);

            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1500;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            WorldCamera = new Camera(GraphicsDevice.Viewport);
            UiCamera = new Camera(new Viewport());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SoundFX.Load(Content);
            Effects.Load(Content);

            var spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(spriteBatch);

            var console = new Console(this);
            Components.Add(console);
            Services.AddService(console);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Runner.Update(gameTime.DeltaTime());
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
