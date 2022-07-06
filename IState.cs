using Microsoft.Xna.Framework;

namespace Gravity
{
    public interface IState
    {
        void Update(Entity entity);
    }

    public class Chasing : IState
    {
        private readonly Hero hero;
        private readonly Pathfinding pathfinding;
        private readonly NavigationGrid navGrid;

        public Chasing(Hero hero)
        {
            this.hero = hero;

            navGrid = new NavigationGrid(hero.Level.Columns, hero.Level.Rows);
            foreach (var cell in hero.Level.Cells)
            {
                if (cell.Solid)
                    navGrid.Solids.Add(cell.Location);
            }

            foreach (var solid in navGrid.Solids)
            {
                foreach (var direction in NavigationGrid.Directions)
                {
                    var neighbour = new Point(solid.X + direction.X, solid.Y + direction.Y);
                    if (navGrid.InBounds(neighbour) &&
                        navGrid.Passable(neighbour))
                        navGrid.NearSolids.Add(neighbour);
                }
            }

            pathfinding = new Pathfinding(navGrid);
        }

        public void Update(Entity entity)
        {
        }
    }

    public class Dead : IState
    {
        public void Update(Entity entity)
        {
        }
    }
}
