using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Gravity
{
    // TODO: GameplayScreen is basically a substitude of Game class.
    // Now instead of entities having a dependency on the whole Game,
    // they will only need GameplayScreen to be added to the game world.
    public class GameplayScreen : GameScreen
    {
        #region Fields
        private ContentManager content;
        private float pauseAlpha;
        private InputAction pauseAction;
        #endregion

        #region Particles
        private SmokeParticleSystem smoke;
        private const float TimeBetweenSmokePuffs = .5f;
        private float timeTillPuff = 0f;
        #endregion

        public Level Level { get; private set; }
        public Hud Hud { get; private set; }
        public Hero Hero { get; set; }

        public readonly Camera WorldCamera = new();
        public readonly Camera UiCamera = new();

        public readonly List<Entity> Entities = new();
        private readonly List<Entity> pendingEntities = new();
        private bool updatingEntities = false;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.P },
                newPressOnly: true);
        }

        public void AddEntity(Entity entity)
        {
            if (updatingEntities)
                pendingEntities.Add(entity);
            else
                Entities.Add(entity);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");

                Level = new Level(content.Load<Texture2D>("Levels/Map1"), this, ScreenManager.Game.Content);
                Hud = new Hud(this);

                Hero = new Hero(this) { Position = new Vector2(100f, 200f) };
                Entities.Add(Hero);

                smoke = new SmokeParticleSystem((Game)ScreenManager.Game, 2);
                ScreenManager.Game.Components.Add(smoke);

                var graphics = (GraphicsDeviceManager)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceManager));
                var centerX = graphics.PreferredBackBufferWidth / 2 - Level.Width / 2;
                var centerY = graphics.PreferredBackBufferHeight / 2 - Level.Height / 2;
                WorldCamera.Position = new Vector2(centerX, centerY);

                // Once the load has finished, we use ResetElapsedTime to tell the game's
                // timining mechanism that we have just finished a very long frame, and
                // that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Unload()
        {
            content.Unload();
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (pauseAction.Evaluate(input, ControllingPlayer, out _))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //// Smoke.
            timeTillPuff -= (float)gameTime.DeltaTime();
            if (timeTillPuff < 0f)
            {
                var where = Vector2.Zero;
                where.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                where.Y = ScreenManager.GraphicsDevice.Viewport.Height / 2;
                smoke.AddParticles(where);
                timeTillPuff = TimeBetweenSmokePuffs;
            }

            updatingEntities = true;
            foreach (var entity in Entities)
                entity.EntityUpdate(gameTime);
            updatingEntities = false;

            foreach (var pending in pendingEntities)
                Entities.Add(pending);
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
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                Color.CornflowerBlue, 0f, 0);

            var spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: WorldCamera.Transform);

            Level.Draw(spriteBatch);

            foreach (var entity in Entities)
                entity.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: UiCamera.Transform);
            Hud.Draw(spriteBatch);
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2f);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
