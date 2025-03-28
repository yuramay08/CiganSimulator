using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CiganSimulator
{
    public class Level
    {
        public string LevelName { get; private set; }
        public List<Platform> Platforms { get; private set; }
        public float Width { get; }
        public float Height { get; }
        public Vector2 SpawnPoint { get; }

        public Level(string name, float width, float height, Vector2 spawnPoint)
        {
            LevelName = name;
            Width = width;
            Height = height;
            SpawnPoint = spawnPoint;
            Platforms = new List<Platform>();
        }

        public void AddPlatform(Platform platform)
        {
            Platforms.Add(platform);
        }

        public void Render() //Fake XD
        {
            foreach (var platform in Platforms)
            {
                Console.WriteLine($"Rendering platform at ({platform.x}, {platform.y}) in level {LevelName}");
            }
        }
        public void Render(int vao, int positionUniformLocation, int scaleUniformLocation)
        {
            foreach (var platform in Platforms)
            {
                // Pass each platform’s position to the shader
                var platformPos = new Vector2(platform.x, platform.y);
                GL.Uniform2(positionUniformLocation, platformPos);

                // Pass each platform’s scale (width and height) to the shader
                var platformScale = new Vector2(platform.width, platform.height);
                GL.Uniform2(scaleUniformLocation, platformScale);

                // Bind the same VAO used for the player
                GL.BindVertexArray(vao);

                // Draw the rectangle geometry
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }
    }
    public static class LevelSetup
    {
        public static LevelManager CreateLevelManager()
        {
            var levelManager = new LevelManager();

            // Level 1: 20x10 world units, for instance

            Level level1 = new Level("L1", 40f, 10f, Vector2.Zero);
            level1.AddPlatform(new Platform(-2, -2.5f, 2, 1));

            

            level1.AddPlatform(new Platform(2, -4, 1, 0.5f));
            level1.AddPlatform(new FinishPlatform(-2, -2.5f, 2, 1, levelManager));

            // Level 2: 30x8 world units
            Level level2 = new Level("L2", 30f, 10f, Vector2.Zero);
            level2.AddPlatform(new Platform(4, -4, 2, 1));
            level2.AddPlatform(new FinishPlatform(6, -4, 2, 1, levelManager));
            
            Level level3 = new Level("L3", 50f, 20f, Vector2.Zero);
            level3.AddPlatform(new Platform(-3.0f, -3.75f, 3, 1));
            level3.AddPlatform(new Platform(1, -2, 2, 1));
            level3.AddPlatform(new FinishPlatform(4, -0.5f, 3, 1, levelManager));

            levelManager.AddLevel(level1);
            levelManager.AddLevel(level2);
            levelManager.AddLevel(level3);
            //levelManager.SelectLevel("L1");
            return levelManager;
        }
    }
}