using Gravity.Coroutines;
using Gravity.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        private readonly List<Entity> entities;
        private readonly List<Entity> pendingEntities;

        private bool updatingEntities = false;
        private Effect flashEffect;
        private CoroutineRunner coroutine;
        private CoroutineHandle spawnHandle;

        public IReadOnlyCollection<Entity> AllEntities
        {
            get { return entities; }
        }

        public GameplayScreen()
        {
            entities = new List<Entity>();
            pendingEntities = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            if (updatingEntities)
                AddOrderedEntity(entity, pendingEntities); //pendingEntities.Add(entity);
            else
                AddOrderedEntity(entity, entities); //entities.Add(entity);
        }

        private static void AddOrderedEntity(Entity entity, List<Entity> collection)
        {
            var order = entity.UpdateOrder;
            var i = 0;
            while (i < collection.Count && order > collection[i].UpdateOrder)
            {
                i++;
            }
            collection.Insert(i, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            for (var i = pendingEntities.Count - 1; i >= 0; i--)
            {
                if (pendingEntities[i] == entity)
                    pendingEntities.RemoveAt(i);
            }

            for (var i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i] == entity)
                    entities.RemoveAt(i);
            }
        }

        public override void LoadContent()
        {
            content ??= new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");

            flashEffect = content.Load<Effect>("Effects/FlashEffect");
            coroutine = ScreenManager.Game.Services.GetService<CoroutineRunner>();

            Level = LevelLoader.Load(content.Load<Texture2D>("Levels/Map1"),
                content.Load<Texture2D>("Textures/Tile"));

            var zoom = 3f;
            GravityGame.UiCamera.Scale = zoom;

            GravityGame.WorldCamera.Position = new Vector2(Level.Width / 2f, Level.Height / 2f);
            GravityGame.WorldCamera.Scale = zoom;

            Hero = new Hero(this) { Position = new Vector2(100f, 25f) };
            AddEntity(Hero);

            AddEntity(new FirePit(this) { Position = new Vector2(Level.Width / 2f, Level.Height - Level.CellSize / 2f) });

            Hud = new Hud(this, Hero);
        }

        public void StartEnemySpawn()
        {
            IEnumerator Spawn()
            {
                var position = new Vector2(Level.Width / 2f, 0f);
                const float spawnInterval = 2f;
                while (true)
                {
                    var roll = Random.FloatValue;
                    if (roll <= .33f)
                        AddEntity(new Bat(this) { Position = position });
                    else if (roll <= .66f)
                        AddEntity(new Zombie(this) { Position = position });
                    else
                        AddEntity(new Demon(this) { Position = position });

                    yield return spawnInterval;
                }
            }
            spawnHandle = coroutine.Run(Spawn());
        }

        public void StopEnemySpawn()
        {
            if (spawnHandle != null)
                spawnHandle.Stop();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        // This is horseshit code. With the current setup Update
        // is called before HandleInput. Everyone knows it's retarded game loop order.
        // Fuck...
        // Or maybe not, fuck if I know, I'm super confused about this. In the book
        // 'User Interface Programming For Games' it's mentioned that enetity update
        // happens before UI input handling. Does UI input handling is separate from
        // entity input handling related to gameplay? Or are those separate things?
        // Will keep this as is for now until I know better.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            updatingEntities = true;

            foreach (var entity in entities)
                entity.HandleInput(input);

            updatingEntities = false;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            updatingEntities = true;

            foreach (var entity in entities)
            {
                if (entity.EntityState == Entity.State.Active)
                    entity.EntityUpdate(gameTime);
            }

            updatingEntities = false;

            foreach (var pending in pendingEntities)
                AddOrderedEntity(pending, entities); //entities.Add(pending);
            pendingEntities.Clear();

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].EntityState == Entity.State.Dead)
                {
                    entities[i].OnDestroy();
                    entities.RemoveAt(i);
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

            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                samplerState: SamplerState.PointClamp,
                transformMatrix: GravityGame.WorldCamera.Transform);

            Level.Draw(spriteBatch);

            foreach (var entity in entities)
            {
                if (!entity.IsFlashing)
                    entity.Draw(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                effect: flashEffect,
                transformMatrix: GravityGame.WorldCamera.Transform);
            foreach (var entity in entities)
            {
                if (entity.IsFlashing)
                {
                    var normalizedColor = entity.FlashColor.ToVector4();
                    flashEffect.Parameters["flash_color"].SetValue(normalizedColor);
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
