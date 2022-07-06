using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Gravity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public Level Level { get; private set; }
        public Hud Hud { get; private set; }
        public Hero Hero { get; set; }

        public readonly Camera WorldCamera = new();
        public readonly Camera UiCamera = new(); // TODO: UI Camera doesn't need shake functionality, should it be refactored?

        public readonly List<Entity> Entities = new();

        private readonly List<Entity> pendingEntities = new();
        private readonly GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        private bool updatingEntities = false;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void AddEntity(Entity entity)
        {
            if (updatingEntities)
                pendingEntities.Add(entity);
            else
                Entities.Add(entity);
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
            Assets.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Effects.Flash.Parameters["flash_color"].SetValue(Vector4.One);

            Level = new Level(Content.Load<Texture2D>("Levels/Map1"), this);

            var centerX = graphics.PreferredBackBufferWidth / 2 - Level.Width / 2;
            var centerY = graphics.PreferredBackBufferHeight / 2 - Level.Height / 2;
            WorldCamera.Position = new Vector2(centerX, centerY);

            //Hero = new Hero(this);
            //Hero.SetCoordinates(50f, 200f);

            Hud = new Hud(this);

            //AddEntity(Hero);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Entity updates
            {
                updatingEntities = true;
                foreach (var entity in Entities)
                    entity.Update(gameTime);
                updatingEntities = false;

                foreach (var entity in pendingEntities)
                    Entities.Add(entity);
                pendingEntities.Clear();

                for (int i = Entities.Count - 1; i >= 0; i--)
                {
                    if (!Entities[i].IsActive)
                    {
                        Entities[i].OnDestroy();
                        Entities.RemoveAt(i);
                    }
                }
            }

            WorldCamera.Update(gameTime);
            UiCamera.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Entities.
            spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: WorldCamera.Transform);
            {
                Level.Draw(spriteBatch);

                foreach (var entity in Entities)
                    entity.Draw(spriteBatch);
            }
            spriteBatch.End();

            // Effects.
            spriteBatch.Begin(SpriteSortMode.BackToFront, effect: Effects.Flash, transformMatrix: WorldCamera.Transform);
            {
                foreach (var entity in Entities)
                {
                    if (entity.IsFlashing)
                    {
                        Effects.Flash.Parameters["flash_color"].SetValue(entity.FlashColor);
                        entity.Draw(spriteBatch);
                    }
                }
            }
            spriteBatch.End();

            // UI.
            spriteBatch.Begin(transformMatrix: UiCamera.Transform);
            {
                Hud.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
