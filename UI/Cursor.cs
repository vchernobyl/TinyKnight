using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.UI
{
    public class Cursor
    {
        private Rectangle rectangle;
        private readonly Color color;

        public int Top
        {
            set { rectangle.Y = value; ; }
        }

        public int Left
        {
            set { rectangle.X = value; }
        }

        public Point Location
        {
            set { rectangle.Location = value; }
        }

        public Cursor(int x, int y, int width, int height, Color color)
        {
            this.rectangle = new Rectangle(x, y, width, height);
            this.color = color;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(rectangle, color);
        }
    }
}
