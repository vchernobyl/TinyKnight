using TinyKnight.Coroutines;
using TinyKnight.Graphics;
using TinyKnight.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TinyKnight
{
    public class TinyKnightGame : Game
    {
        public static Camera WorldCamera { get; private set; }
        public static Camera UiCamera { get; private set; }

        public static CoroutineRunner Coroutine { get; private set; }

        private readonly GraphicsDeviceManager graphics;
        private readonly ScreenManager screenManager;

        public TinyKnightGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Services.AddService(screenManager = new ScreenManager(this));
            Services.AddService(Coroutine = new CoroutineRunner());

            Components.Add(screenManager);

#if DEBUG
            Components.Add(new FrameRateCounter(this));
#endif

            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());
            screenManager.AddScreen(new GameplayScreen());
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1500;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            WorldCamera = new Camera(GraphicsDevice.Viewport);
            UiCamera = new Camera(new Viewport());

            DebugRenderer.Initialize(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var spriteBatch = new SpriteBatch(GraphicsDevice);
            DebugRenderer.Initialize(GraphicsDevice);
            Services.AddService(spriteBatch);

            var console = new Console(this);
            Components.Add(console);
            Services.AddService(console);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Coroutine.Update(gameTime.DeltaTime());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            DebugRenderer.Draw(gameTime, view: WorldCamera.Transform);
        }
    }
}
