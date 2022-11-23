using Gravity.Coroutines;
using Gravity.Entities;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        }

        public void AddEntity(Entity entity)
        {
            var order = entity.UpdateOrder;
            var i = 0;
            while (i < entities.Count && order > entities[i].UpdateOrder)
            {
                i++;
            }
            entities.Insert(i, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public override void LoadContent()
        {
            content ??= new ContentManager(ScreenManager.Game.Services, rootDirectory: "Content");

            flashEffect = content.Load<Effect>("Effects/FlashEffect");
            coroutine = ScreenManager.Game.Services.GetService<CoroutineRunner>();

            Level = LevelLoader.Load(content.Load<Texture2D>("Levels/Map1"),
                content.Load<Texture2D>("Textures/Tile"));

            var zoom = 3f;
            GravityGame.WorldCamera.Position = new Vector2(Level.Width / 2f, Level.Height / 2f);
            GravityGame.WorldCamera.Scale = zoom;

            Hero = new Hero(this) { Position = new Vector2(100f, 25f) };
            AddEntity(Hero);

            AddEntity(new FirePit(this, new Vector2(Level.Width / 2f, (Level.Height - Level.CellSize / 2f) + 15f)));

            Hud = new Hud(this);

            coroutine.Run(SpawnChest(), delay: 5f);
        }

        public void StartEnemySpawn()
        {
            IEnumerator Spawn()
            {
                var position = new Vector2(Level.Width / 2f, 0f);
                const float spawnInterval = 1f;
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
            var copy = new List<Entity>(entities);
            foreach (var entity in copy)
                entity.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            var entityCopy = new List<Entity>(entities);
            foreach (var entity in entityCopy)
            {
                if (entity.EntityState == Entity.State.Active)
                    entity.EntityUpdate(gameTime);
                else if (entity.EntityState == Entity.State.Dead)
                {
                    entity.OnDestroy();
                    entities.Remove(entity);
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

            DebugRenderer.Draw(gameTime, view: GravityGame.WorldCamera.Transform);
        }

        private IEnumerator SpawnChest()
        {
            while (true)
            {
                // Choose a non-solid cell to spawn the chest from.
                Cell PickCell()
                {
                    var column = Random.IntRange(0, Level.Columns);
                    var row = Random.IntRange(0, Level.Rows);
                    return Level.Cells[column, row];
                }

                var randomCell = PickCell();
                while (randomCell.Solid)
                    randomCell = PickCell();

                var chest = new Chest(this) { Position = randomCell.Position };
                AddEntity(chest);

                yield return 15f;
            }
        }
    }
}
