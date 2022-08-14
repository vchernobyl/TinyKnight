using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Gravity
{
    public class PortalSpawner
    {
        private readonly uint maxActivePortals;
        private readonly List<(Vector2, Portal.EnemyType)> possiblePortalPlacements;
        private readonly List<Portal> activePortals = new List<Portal>();
        private readonly GameplayScreen gameplayScreen;

        private uint currentlyActivePortals = 0;

        public PortalSpawner(GameplayScreen gameplayScreen,
            List<(Vector2, Portal.EnemyType)> possiblePortalPlacements,
            uint maxActivePortals,
            uint activePortalsOnStart)
        {
            this.gameplayScreen = gameplayScreen;
            this.maxActivePortals = maxActivePortals;
            this.possiblePortalPlacements = possiblePortalPlacements;

            for (int i = 0; i < activePortalsOnStart; i++)
            {
                var (position, enemyType) = PickPortalPlacement();
                var portal = new Portal(position, gameplayScreen, enemyType);
                gameplayScreen.AddEntity(portal);
            }
        }

        public (Vector2, Portal.EnemyType) PickPortalPlacement()
        {
            var randomPlacement = Numerics.PickOne(possiblePortalPlacements);
            var activePositions = gameplayScreen
                .Entities
                .Where(e => e is Portal)
                .Select(e => e.Position);

            while (activePositions.Contains(randomPlacement.Item1))
                randomPlacement = Numerics.PickOne(possiblePortalPlacements);

            return randomPlacement;
        }
    }
}
