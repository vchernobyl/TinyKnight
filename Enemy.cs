using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Enemy : Entity
    {
        public Enemy(Texture2D texture, Level level) : base(texture, level)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
