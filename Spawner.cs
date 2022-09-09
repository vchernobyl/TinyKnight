using Gravity.Entities;
using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Spawner
    {
        private readonly Vector2 position;
        private readonly GameplayScreen gameplayScreen;

        private const float SpawnInterval = 2f;
        private float time;

        public Spawner(Vector2 position, GameplayScreen gameplayScreen)
        {
            this.position = position;
            this.gameplayScreen = gameplayScreen;
        }

        public void Update(GameTime gameTime)
        {
            time += gameTime.DeltaTime();
            if (time >= SpawnInterval)
            {
                time = 0f;

                var roll = Random.FloatValue;
                if (roll <= .33f)
                    gameplayScreen.AddEntity(new Zombie(gameplayScreen) { Position = position });
                else if (roll <= .66f)
                    gameplayScreen.AddEntity(new Skeleton(gameplayScreen) { Position = position });
                else
                    gameplayScreen.AddEntity(new Bat(gameplayScreen) { Position = position });
            }
        }
    }
}
