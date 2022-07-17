using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gravity
{
    // TODO: GameplayScreen is basically a substitude of Game class.
    // Now instead of entities having a dependency on the whole Game,
    // they will only need GameplayScreen to be added to the game world.
    public class GameplayScreen : GameScreen
    {
        private ContentManager content;

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
        }

        public void AddEntity(Entity entity)
        {
            if (updatingEntities)
                pendingEntities.Add(entity);
            else
                Entities.Add(entity);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");

            Level = new Level(content.Load<Texture2D>("Levels/Map1"), this, ScreenManager.Game.Content);
            Hud = new Hud(this);

            Hero = new Hero(this) { Position = new Vector2(100f, 200f) };
            Entities.Add(Hero);

            var centerX = ScreenManager.GraphicsDevice.Viewport.Width / 2 - Level.Width / 2;
            var centerY = ScreenManager.GraphicsDevice.Viewport.Height / 2 - Level.Height / 2;
            WorldCamera.Position = new Vector2(centerX, centerY);

            // Once the load has finished, we use ResetElapsedTime to tell the game's
            // timining mechanism that we have just finished a very long frame, and
            // that it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

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
            {
                if (!entity.IsFlashing)
                    entity.Draw(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin(effect: Effects.Flash, transformMatrix: WorldCamera.Transform);
            foreach (var entity in Entities)
            {
                if (entity.IsFlashing)
                {
                    Effects.Flash.Parameters["flash_color"].SetValue(entity.FlashColor);
                    entity.Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: UiCamera.Transform);
            Hud.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
