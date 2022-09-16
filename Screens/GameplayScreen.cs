using Gravity.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Gravity
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager content;

        public Level Level { get; private set; }
        public Hud Hud { get; private set; }
        public Hero Hero { get; private set; }

        public readonly List<Entity> Entities = new List<Entity>();
        private readonly List<Entity> pendingEntities = new List<Entity>();

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
            content ??= new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");

            Level = LevelLoader.Load(content.Load<Texture2D>("Levels/Map1"),
                content.Load<Texture2D>("Textures/Tile"));

            var zoom = 3f;
            GravityGame.UiCamera.Scale = zoom;

            GravityGame.WorldCamera.Position = new Vector2(Level.Width / 2f, Level.Height / 2f);
            GravityGame.WorldCamera.Scale = zoom;

            Hero = new Hero(this) { Position = new Vector2(50f, 25f) };
            Entities.Add(Hero);

            Hud = new Hud(this, Hero);

            var position = new Vector2(Level.Width / 2f, 0f);
            const float spawnInterval = 2f;

            IEnumerator Spawn()
            {
                while (true)
                {
                    var roll = Random.FloatValue;
                    if (roll <= .33f)
                        AddEntity(new Bat(this) { Position = position });
                    else if (roll <= .66f)
                        AddEntity(new Zombie(this) { Position = position });
                    else
                        AddEntity(new Skeleton(this) { Position = position });

                    yield return spawnInterval;
                }
            }

            //GravityGame.Runner.Run(Spawn());

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
            {
                if (entity.EntityState == Entity.State.Active)
                    entity.EntityUpdate(gameTime);
            }

            foreach (var entity in Entities)
            {
                if (entity.EntityState == Entity.State.Active)
                    entity.PostUpdate(gameTime);
            }
            updatingEntities = false;

            foreach (var pending in pendingEntities)
                Entities.Add(pending);
            pendingEntities.Clear();

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                if (Entities[i].EntityState == Entity.State.Dead)
                {
                    Entities[i].OnDestroy();
                    Entities.RemoveAt(i);
                }
            }

            GravityGame.WorldCamera.Update(gameTime);
            GravityGame.UiCamera.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                Color.Black, 0f, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                samplerState: SamplerState.PointClamp,
                transformMatrix: GravityGame.WorldCamera.Transform);

            Level.Draw(spriteBatch);

            foreach (var entity in Entities)
            {
                if (!entity.IsFlashing)
                    entity.Draw(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                effect: Effects.Flash,
                transformMatrix: GravityGame.WorldCamera.Transform);
            foreach (var entity in Entities)
            {
                if (entity.IsFlashing)
                {
                    var normalizedColor = entity.FlashColor.ToVector4();
                    Effects.Flash.Parameters["flash_color"].SetValue(normalizedColor);
                    entity.Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: GravityGame.UiCamera.Transform);
            Hud.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
