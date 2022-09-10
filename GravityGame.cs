using Gravity.Coroutines;
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

        public static readonly CoroutineRunner Runner = new CoroutineRunner();

        private bool paused = false;

        public GravityGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Add initial screens.
            //screenManager.AddScreen(new SandboxScreen());
            //screenManager.AddScreen(new ParticlesScreen());
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
            Textures.Load(Content);
            SoundFX.Load(Content);
            Fonts.Load(Content);
            Effects.Load(Content);

            var spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Input.WasKeyPressed(Keys.P))
                paused = !paused;

            if (!paused)
            {
                Runner.Update(gameTime.DeltaTime());
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
