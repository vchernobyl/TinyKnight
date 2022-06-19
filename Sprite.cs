using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Gravity
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public Rectangle Source { get; set; }
        public SpriteEffects Flip { get; set; }

        private float layerDepth;
        public float LayerDepth
        {
            get { return layerDepth; }
            set
            {
                Debug.Assert(value >= 0f && layerDepth <= 1f);
                layerDepth = value;
            }
        }

        private readonly Texture2D texture;

        public Sprite(Texture2D texture)
        {
            this.texture = texture;

            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Rotation = 0f;
            Source = texture.Bounds;
            Flip = SpriteEffects.None;
            LayerDepth = .5f;
        }

        public void Draw(SpriteBatch batch)
        {
            var center = new Vector2(Position.X - Level.CellSize / 2, Position.Y - Level.CellSize / 2);
            batch.Draw(texture, center, Source, Color.White, Rotation, Origin, Scale, Flip, LayerDepth);
        }
    }
}
