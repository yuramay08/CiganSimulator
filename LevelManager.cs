using System;
using System.Collections.Generic;

namespace CiganSimulator
{
    public class LevelManager
    {
        private Dictionary<string, Level> levels = new Dictionary<string, Level>();
        public Level CurrentLevel { get; private set; }

        public void AddLevel(Level level)
        {
            levels[level.LevelName] = level;
        }

        public void SelectLevel(string levelName)
        {
            if (levels.ContainsKey(levelName))
            {
                CurrentLevel = levels[levelName];
                Console.WriteLine($"Selected level: {levelName}");
            }
            else
            {
                Console.WriteLine($"Level {levelName} not found.");
            }
        }
    }
}