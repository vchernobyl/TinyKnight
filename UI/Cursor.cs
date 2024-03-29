﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight.UI
{
    public class Cursor
    {
        private Rectangle rectangle;
        private Color color;
        
        private float blinkTime;
        private readonly float blinkDuration;
        private readonly Texture2D texture;

        private bool toggle;

        public int Top
        {
            get { return rectangle.Top; }
            set { rectangle.Y = value; ; }
        }

        public int Left
        {
            get { return rectangle.Left; }
            set { rectangle.X = value; }
        }

        public Point Location
        {
            get { return rectangle.Location; }
            set { rectangle.Location = value; }
        }

        public Cursor(Texture2D texture, int x, int y, int width, int height,
            Color color, float blinkRate = 1f)
        {
            this.texture = texture;
            this.rectangle = new Rectangle(x, y, width, height);
            this.color = color;
            this.blinkTime = 0f;
            this.blinkDuration = blinkRate;
            this.toggle = true;
        }

        public void PauseBlink()
        {
            blinkTime = blinkDuration;
            toggle = true;
        }

        public void Update(GameTime gameTime)
        {
            blinkTime -= gameTime.DeltaTime();
            if (blinkTime <= 0f)
            {
                blinkTime = blinkDuration;
                toggle = !toggle;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, toggle ? color : Color.Transparent);
        }
    }
}
