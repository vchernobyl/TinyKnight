using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Sprite : Drawable
    {
        public Point Size => texture.Bounds.Size;

        private readonly Texture2D texture;

        public Sprite(Texture2D texture)
        {
            this.texture = texture;

            Origin = texture.Bounds.Center.ToVector2();
            Scale = Vector2.One;
            Rotation = 0f;
            Source = texture.Bounds;
            Flip = SpriteEffects.None;
            LayerDepth = .5f;
            Color = Color.White;
        }

        public override void Draw(SpriteBatch batch)
        {
            // TODO: We can use scale instead of separate field
            // for slipping sprites.
            // Finish this.
            var flip = SpriteEffects.None;
            if (Scale.X < 0)
                flip |= SpriteEffects.FlipVertically;
            if (Scale.Y < 0)
                flip |= SpriteEffects.FlipHorizontally;

            batch.Draw(texture, Position, Source, Color, Rotation, Origin, Scale, Flip, LayerDepth);
        }
    }
}
