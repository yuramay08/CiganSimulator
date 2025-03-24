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

        public Level(string levelName)
        {
            LevelName = levelName;
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
            // Instantiate level manager
            LevelManager levelManager = new LevelManager();

            // Define your levels
            Level level1 = new Level("L1");
            level1.AddPlatform(new Platform(-2, -4, 2, 1));
            level1.AddPlatform(new Platform(2, -4, 2, 1));

            Level level2 = new Level("L2");
            level2.AddPlatform(new Platform(4, -4, 2, 1));
            level2.AddPlatform(new Platform(6, -4, 2, 1));

            // Add them and pick the active one
            levelManager.AddLevel(level1);
            levelManager.AddLevel(level2);
            levelManager.SelectLevel("L1");

            return levelManager;
        }
    }
}