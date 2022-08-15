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

        private int CurrentlyActivePortals => gameplayScreen.Entities
            .Count(e => e is Portal);

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
            var activePositions = gameplayScreen.Entities
                .Where(e => e is Portal)
                .Select(e => e.Position);

            var position = randomPlacement.Item1;
            var dist = Vector2.Distance(gameplayScreen.Hero.Position, position);
            // TODO: Make sure the position of the new portal is not the same
            // as the one that just got destroyed.
            while (activePositions.Contains(position))
                randomPlacement = Numerics.PickOne(possiblePortalPlacements);

            return randomPlacement;
        }

        public void Update(GameTime gameTime)
        {
            var diff = maxActivePortals - CurrentlyActivePortals;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    var (position, type) = PickPortalPlacement();
                    var portal = new Portal(position, gameplayScreen, type);
                    gameplayScreen.AddEntity(portal);
                }
            }
        }
    }
}
