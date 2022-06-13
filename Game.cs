using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Gravity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public Level Level { get; private set; }

        private SpriteBatch spriteBatch;
        private Spawner spawner;

        private readonly List<Entity> entities = new();

        public Game()
        {
            new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Level = new Level(Content.Load<Texture2D>("Levels/Map1"), Services);
            spawner = new Spawner(Level.GetSpawnPosition(), this);

            var hero = new Hero(Content.Load<Texture2D>("Textures/character_0000"), Level);
            hero.SetCoordinates(50f, 50f);
            AddEntity(hero);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var entity in entities)
            {
                entity.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            Level.Draw(spriteBatch);
            foreach (var entity in entities)
            {
                entity.Draw(spriteBatch);
            }
            spawner.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
