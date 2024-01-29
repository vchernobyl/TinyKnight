using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight
{
    public class Subtexture
    {
        public readonly Texture2D Texture;
        public readonly Rectangle Source;

        public Subtexture(Texture2D texture, Rectangle source)
        {
            Texture = texture;
            Source = source;
        }
    }
}
