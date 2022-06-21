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
        public Hero Hero { get; private set; }

        public Camera Camera { get; } = new Camera();

        public readonly List<Entity> Entities = new();

        private readonly List<Entity> pendingEntities = new();
        private readonly List<Spawner> spawners = new();
        private readonly GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        private Effect flash;
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
            graphics.PreferredBackBufferWidth = 1020;
            graphics.PreferredBackBufferHeight = 780;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            flash = Content.Load<Effect>("Effects/FlashEffect");
            flash.Parameters["flash_color"].SetValue(Vector4.One);

            Level = new Level(Content.Load<Texture2D>("Levels/Map1"), Services);

            var sprite = new Sprite(Content.Load<Texture2D>("Textures/character_0000"));
            Hero = new Hero(this, sprite);
            Hero.SetCoordinates(50f, 100f);

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

            AddEntity(Hero);
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

            Camera.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Regular drawing.
            spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: Camera.Transform);
            {
                Level.Draw(spriteBatch);

                foreach (var entity in Entities)
                    entity.Draw(spriteBatch);

                foreach (var spawner in spawners)
                    spawner.Draw(spriteBatch);

                Hud.Draw(spriteBatch);
            }
            spriteBatch.End();

            // Drawing with effect.
            spriteBatch.Begin(SpriteSortMode.BackToFront, effect: flash, transformMatrix: Camera.Transform);
            {
                foreach (var entity in Entities)
                {
                    if (entity.IsFlashing)
                        entity.Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
