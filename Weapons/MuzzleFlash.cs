using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class MuzzleFlash : Entity
    {
        public bool Enabled { get; set; }

        private readonly Timer timer;

        public MuzzleFlash(GameplayScreen gameplayScreen)
            : base(gameplayScreen, new Sprite(Textures.MuzzleFlash))
        {
            sprite.LayerDepth = .1f;
            timer = new Timer(duration: .0125f, () => { Enabled = false; });
            timer.Start();
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            if (Enabled)
                base.Draw(batch);
        }
    }
}
