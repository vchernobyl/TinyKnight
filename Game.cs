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

        #region Particles
        private SmokeParticleSystem smoke;
        private const float TimeBetweenSmokePuffs = .5f;
        private float timeTillPuff = 0f;
        #endregion

        public SpriteBatch SpriteBatch { get; private set; }
        private bool updatingEntities = false;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            smoke = new SmokeParticleSystem(this, 2);
            Components.Add(smoke);
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

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Effects.Flash.Parameters["flash_color"].SetValue(Vector4.One);

            Level = new Level(Content.Load<Texture2D>("Levels/Map1"), this);

            var centerX = graphics.PreferredBackBufferWidth / 2 - Level.Width / 2;
            var centerY = graphics.PreferredBackBufferHeight / 2 - Level.Height / 2;
            WorldCamera.Position = new Vector2(centerX, centerY);

            Hero = new Hero(this) { Position = new Vector2(100f, 200f) };
            Entities.Add(Hero);

            Hud = new Hud(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Smoke.
            timeTillPuff -= (float)gameTime.DeltaTime();
            if (timeTillPuff < 0f)
            {
                var where = Vector2.Zero;
                where.X = graphics.GraphicsDevice.Viewport.Width / 2;
                where.Y = graphics.GraphicsDevice.Viewport.Height / 2;
                smoke.AddParticles(where);
                timeTillPuff = TimeBetweenSmokePuffs;
            }

            updatingEntities = true;
            foreach (var entity in Entities)
                entity.EntityUpdate(gameTime);
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

            WorldCamera.Update(gameTime);
            UiCamera.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Entities.
            SpriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: WorldCamera.Transform);
            {
                Level.Draw(SpriteBatch);

                foreach (var entity in Entities)
                    entity.Draw(SpriteBatch);
            }
            SpriteBatch.End();

            // Effects.
            SpriteBatch.Begin(SpriteSortMode.BackToFront, effect: Effects.Flash, transformMatrix: WorldCamera.Transform);
            {
                foreach (var entity in Entities)
                {
                    if (entity.IsFlashing)
                    {
                        Effects.Flash.Parameters["flash_color"].SetValue(entity.FlashColor);
                        entity.Draw(SpriteBatch);
                    }
                }
            }
            SpriteBatch.End();

            // UI.
            SpriteBatch.Begin(transformMatrix: UiCamera.Transform);
            {
                Hud.Draw(SpriteBatch);
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
