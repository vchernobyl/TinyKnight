using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public Level Level { get; private set; }
        public Hud Hud { get; private set; }

        public readonly List<Entity> Entities = new();

        private readonly List<Entity> pendingEntities = new();
        private readonly List<Spawner> spawners = new();

        private SpriteBatch spriteBatch;
        private bool updatingEntities = false;

        public Game()
        {
            _ = new GraphicsDeviceManager(this);
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

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Level = new Level(Content.Load<Texture2D>("Levels/Map1"), Services);
            Hud = new Hud(this);

            foreach (var position in Level.GetSpawnPositions())
            {
                var spawner = new Spawner(position, this)
                {
                    MaxEntities = 5,
                    DelayBetweenSpawns = 1f,
                };
                spawners.Add(spawner);
            }

            var sprite = new Sprite(Content.Load<Texture2D>("Textures/character_0000"));
            var hero = new Hero(this, sprite, Level);
            hero.SetCoordinates(50f, 100f);
            AddEntity(hero);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
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

            foreach (var spawner in spawners)
                spawner.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront);
            {
                Level.Draw(spriteBatch);

                foreach (var entity in Entities)
                    entity.Draw(spriteBatch);

                foreach (var spawner in spawners)
                    spawner.Draw(spriteBatch);

                Hud.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
