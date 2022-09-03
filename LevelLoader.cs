using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public static class LevelLoader
    {
        public static Level Load(Texture2D levelLayer, Texture2D cellTexture)
        {
            var levelData = GetPixels(levelLayer);
            var level = new Level(levelLayer.Width, levelLayer.Height, cellTexture);

            // Generate level out of the image data.
            for (int y = 0; y < level.Rows; y++)
            {
                for (int x = 0; x < level.Columns; x++)
                {
                    var pixel = levelData[x, y];
                    var type = pixel switch
                    {
                        var _ when pixel == Color.White => Cell.CellType.Wall,
                        _ => Cell.CellType.Empty,
                    };

                    var cell = new Cell(x, y, type, type == Cell.CellType.Wall);
                    level.Cells[x, y] = cell;
                }
            }

            return level;
        }

        private static Color[,] GetPixels(Texture2D levelLayer)
        {
            // Load texture pixels into a 1-dimensional array.
            var pixels = new Color[levelLayer.Width * levelLayer.Height];
            levelLayer.GetData(pixels);

            // Store pixels into a 2-dimensional array.
            // TODO: Check later if we can simplify just by using 1-dimensional array everywhere.
            var levelData = new Color[levelLayer.Width, levelLayer.Height];
            for (int y = 0; y < levelLayer.Height; y++)
            {
                for (int x = 0; x < levelLayer.Width; x++)
                {
                    levelData[x, y] = pixels[x + y * levelLayer.Width];
                }
            }

            return levelData;
        }
    }
}
