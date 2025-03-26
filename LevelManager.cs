using System;
using System.Collections.Generic;
using System.Numerics;

namespace CiganSimulator
{
    public class LevelManager
    {
        private Dictionary<string, Level> levels = new Dictionary<string, Level>();
        private List<string> levelOrder = new List<string>();
        public Level? CurrentLevel { get; private set; }
    
        public void AddLevel(Level level)
        {
            levels[level.LevelName] = level;
            levelOrder.Add(level.LevelName);
        }

        public void SelectLevel(string levelName, ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            if (levels.ContainsKey(levelName))
            {
                CurrentLevel = levels[levelName];
                playerPosition = new OpenTK.Mathematics.Vector2(-6.0f + 3.0f / 6.0f, -4.5f);

                Console.WriteLine($"Selected level: {levelName}");
            }
            else
            {
                Console.WriteLine($"Level {levelName} not found.");
            }
        }

        public void GoToNextLevel(ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            int currentLevelIndex = levelOrder.IndexOf(CurrentLevel.LevelName);

            if (currentLevelIndex < levelOrder.Count - 1)
            {
                currentLevelIndex++;
                SelectLevel(levelOrder[currentLevelIndex], ref playerPosition);
            }
            else
            {
                Console.WriteLine("Completed all levels");
            }
        }
    }
}