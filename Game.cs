﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch? spriteBatch;
        public SpriteBatch SpriteBatch => spriteBatch!;

        private readonly GraphicsDeviceManager graphics;
        private readonly ScreenManager screenManager;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Add initial screens.
            screenManager.AddScreen(new ParticlesScreen());
            //screenManager.AddScreen(new BackgroundScreen());
            //screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1500;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Textures.Load(Content);
            SoundFX.Load(Content);
            Fonts.Load(Content);
            Effects.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
