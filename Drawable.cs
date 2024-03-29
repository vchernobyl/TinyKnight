﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight
{
    public abstract class Drawable
    {
        public Vector2 Position { get; set; }
        public Vector2 Origin;
        public Vector2 Scale;
        public float Rotation { get; set; }
        public float LayerDepth { get; set; }
        public Rectangle Source { get; set; }
        public SpriteEffects Flip { get; set; }
        public Color Color { get; set; }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
