using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Gravity
{
    public class PortalSpawner
    {
        private readonly uint maxActivePortals;
        private readonly List<(Vector2, Portal.EnemyType)> possiblePortalPlacements;
        private readonly GameplayScreen gameplayScreen;

        private float time = 0f;
        private const float Delay = 3f;

        public PortalSpawner(GameplayScreen gameplayScreen,
            List<(Vector2, Portal.EnemyType)> possiblePortalPlacements,
            uint maxActivePortals)
        {
            this.gameplayScreen = gameplayScreen;
            this.maxActivePortals = maxActivePortals;
            this.possiblePortalPlacements = possiblePortalPlacements;
        }

        public (Vector2, Portal.EnemyType) PickPortalPlacement()
        {
            var randomPlacement = Numerics.PickOne(possiblePortalPlacements);
            var activePositions = gameplayScreen.Entities
                .Where(e => e is Portal)
                .Select(e => e.Position);

            while (activePositions.Contains(randomPlacement.Item1))
                randomPlacement = Numerics.PickOne(possiblePortalPlacements);

            return randomPlacement;
        }

        public void Update(GameTime gameTime)
        {
            time += gameTime.DeltaTime();
            if (time >= Delay)
            {
                time = 0f;

                // Spawn a single portal.
                var activePortals = gameplayScreen.Entities.Count(e => e is Portal);
                var diff = maxActivePortals - activePortals;
                if (diff > 0)
                {
                    var (position, type) = PickPortalPlacement();
                    var portal = new Portal(position, gameplayScreen, type);
                    gameplayScreen.AddEntity(portal);
                }
            }
        }
    }
}
