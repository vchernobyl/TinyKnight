using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public float LayerDepth { get; set; }
        public Rectangle Source { get; set; }
        public SpriteEffects Flip { get; set; }
        public Color Color { get; set; }

        public Point Size => texture.Bounds.Size;

        private readonly Texture2D texture;

        public Sprite(Texture2D texture)
        {
            this.texture = texture;

            Origin = texture.Bounds.Center.ToVector2();
            Scale = 1f;
            Rotation = 0f;
            Source = texture.Bounds;
            Flip = SpriteEffects.None;
            LayerDepth = .5f;
            Color = Color.White;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, Position, Source, Color, Rotation, Origin, Scale, Flip, LayerDepth);
        }
    }
}
