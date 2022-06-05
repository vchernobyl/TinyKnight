using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class LevelManager
    {
        private readonly string[] levels =
        {
            "Map1",
            "Map2",
            "Map3",
        };

        private int currentLevelIndex = 0;
        private readonly ContentManager content;

        public Level CurrentLevel { get; private set; }

        public LevelManager(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, rootDirectory: "Content");
            LoadLevel();
        }

        public void NextLevel()
        {
            currentLevelIndex = (currentLevelIndex + 1) % levels.Length;
            LoadLevel();
        }

        public void SetLevel(int levelIndex)
        {
            currentLevelIndex = levelIndex;
            LoadLevel();
        }

        private void LoadLevel()
        {
            var levelData = content.Load<Texture2D>($"Levels/{levels[currentLevelIndex]}");
            CurrentLevel = new Level(levelData, this, content.ServiceProvider);
        }
    }
}
