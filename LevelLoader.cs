using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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

        // TODO: To populate the level we don't need an instance of the level, because
        // we are not going to add anything to the cells. Cells are only for collisions
        // and tile rendering.
        //
        // We could process the entityLayer texture, capture positions and types of all
        // entities present and add those to the gameplay screen.
        public static void GetEntities(GameplayScreen gameplayScreen, Texture2D entityLayer)
        {
            var entityData = GetPixels(entityLayer);

            for (int y = 0; y < entityLayer.Height; y++)
            {
                for (int x = 0; x < entityLayer.Width; x++)
                {
                    var pixel = entityData[x, y];
                    var position = new Vector2(x * Level.CellSize, y * Level.CellSize);

                    if (pixel == Color.Red)
                    {
                        var portal = new Portal(position, gameplayScreen, Portal.EnemyType.Flyer);
                        gameplayScreen.AddEntity(portal);
                    }
                    if (pixel == Color.Yellow)
                    {
                        var portal = new Portal(position, gameplayScreen, Portal.EnemyType.Walker);
                        gameplayScreen.AddEntity(portal);
                    }
                }
            }
        }

        public static List<(Vector2, Portal.EnemyType)> GetPortals(Texture2D entityLayer)
        {
            var portals = new List<(Vector2, Portal.EnemyType)>();
            var entityData = GetPixels(entityLayer);

            for (int y = 0; y < entityLayer.Height; y++)
            {
                for (int x = 0; x < entityLayer.Width; x++)
                {
                    var pixel = entityData[x, y];
                    var position = new Vector2(x * Level.CellSize, y * Level.CellSize);
                    if (pixel == Color.Red)
                        portals.Add((position, Portal.EnemyType.Flyer));
                    if (pixel == Color.Yellow)
                        portals.Add((position, Portal.EnemyType.Walker));
                }
            }

            return portals;
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
