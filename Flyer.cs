using Microsoft.Xna.Framework;

namespace Gravity
{
    public class Flyer : Entity
    {
        private readonly Hero hero;

        public Flyer(Game game) : base(game, new Sprite(Textures.Flyer))
        {
            hero = game.Hero;
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Use A* pathfinding to simulate the fly-path to chase the hero.
            base.Update(gameTime);
        }
    }
}
