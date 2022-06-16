using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public Rectangle Source { get; set; }

        private readonly Texture2D texture;

        public Sprite(Texture2D texture)
        {
            this.texture = texture;
            Scale = Vector2.One;
            Origin = Vector2.Zero;
            Source = texture.Bounds;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, Position, Source, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
