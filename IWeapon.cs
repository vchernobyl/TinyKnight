using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public interface IWeapon
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch batch);
    }
}
