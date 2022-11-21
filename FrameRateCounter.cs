using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class FrameRateCounter : DrawableGameComponent
    {
        private readonly ContentManager content;

        private SpriteBatch batch;
        private SpriteFont font;

        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game) : base(game)
        {
            content = new ContentManager(game.Services, rootDirectory: "Content");
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("Fonts/Default");
        }

        protected override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            var fps = $"fps: {frameRate}";

            batch.Begin();

            batch.DrawString(font, fps, new Vector2(33, 33), Color.Black);
            batch.DrawString(font, fps, new Vector2(32, 32), Color.White);

            batch.End();
        }
    }
}
