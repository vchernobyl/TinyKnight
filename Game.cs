using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Gravity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public Level Level { get; private set; }
        public readonly List<Entity> Entities = new();

        private SpriteBatch spriteBatch;
        private readonly List<Spawner> spawners = new();

        public Game()
        {
            _ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Level = new Level(Content.Load<Texture2D>("Levels/Map1"), Services);

            foreach (var position in Level.GetSpawnPositions())
            {
                var spawner = new Spawner(position, this)
                {
                    MaxEntities = 5,
                    DelayBetweenSpawns = 1f,
                };
                spawners.Add(spawner);
            }

            var hero = new Hero(this, Content.Load<Texture2D>("Textures/character_0000"), Level);
            hero.SetCoordinates(50f, 100f);
            AddEntity(hero);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var entity in Entities)
                entity.Update(gameTime);

            foreach (var spawner in spawners)
                spawner.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            Level.Draw(spriteBatch);

            foreach (var entity in Entities)
                entity.Draw(spriteBatch);

            foreach (var spawner in spawners)
                spawner.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
