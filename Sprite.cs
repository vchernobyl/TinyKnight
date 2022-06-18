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
        public SpriteEffects Effect { get; set; }
        public float LayerDepth { get; set; }

        private readonly Texture2D texture;

        public Sprite(Texture2D texture)
        {
            this.texture = texture;
            Scale = Vector2.One;
            Origin = Vector2.Zero;
            Source = texture.Bounds;
            Effect = SpriteEffects.None;
            LayerDepth = .5f;
        }

        public void Draw(SpriteBatch batch)
        {
            var center = new Vector2(Position.X - Level.CellSize / 2, Position.Y - Level.CellSize / 2);
            batch.Draw(texture, center, Source, Color.White, Rotation, Origin, Scale, Effect, LayerDepth);
        }
    }
}
