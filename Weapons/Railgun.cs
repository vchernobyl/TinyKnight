using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Beam : Entity
    {
        public bool Enabled { get; set; }

        public Beam(GameplayScreen gameplayScreen)
            : base(gameplayScreen, new Sprite(Textures.Pixel))
        {
        }

        public override void Draw(SpriteBatch batch)
        {
            if (Enabled)
                base.Draw(batch);
        }
    }

    public class Railgun : Weapon
    {
        private readonly Beam beam;

        public Railgun(GameplayScreen gameplayScreen, Hero hero)
            : base(hero, fireRate: .3f, name: "Railgun")
        {
            beam = new Beam(gameplayScreen) { Enabled = false };
            gameplayScreen.AddEntity(beam);
        }

        public override void Shoot()
        {
            var position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;
            beam.Position = position;
            beam.Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (beam.Enabled)
                beam.Position = hero.Position + Vector2.UnitX * hero.Facing * Level.CellSize;

            base.Update(gameTime);
        }
    }
}
