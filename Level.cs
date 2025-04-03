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

            //MAX JUMP LENGTH = 15
            //MAX JUMP HEIGHT = 1.75


            // Level 1
            Level level1 = new Level("L1", 100f, 10f, Vector2.Zero);
            level1.AddPlatform(new Void(0, -4.75f, level1.Width, 0.5f, levelManager)); //neviem preco to nezabere celu sirku levelu
            level1.AddPlatform(new Platform(-4.25f, -4, 5, 5));

            level1.AddPlatform(new Platform(3, -2, 2, 1));
            level1.AddPlatform(new Platform(10, -2.5f, 4, 1));  
            level1.AddPlatform(new Platform(20, -1.5f, 4, 1));  
            level1.AddPlatform(new Platform(26, -2, 3, 1));  
            level1.AddPlatform(new Platform(30, -3.5f, 3, 1));  
            level1.AddPlatform(new Platform(35, -2f, 2, 1));  
            level1.AddPlatform(new Platform(42, -2.5f, 3, 1));  
            level1.AddPlatform(new Platform(48, -3, 3, 1));  
            level1.AddPlatform(new Platform(55, -1.5f, 4, 1));  
            level1.AddPlatform(new Platform(62, -3.5f, 3, 1));  
            level1.AddPlatform(new Platform(68, -2.5f, 4, 1));  
            level1.AddPlatform(new Platform(75, -1.5f, 3, 1));  

            level1.AddPlatform(new FinishPlatform(80, -3, 5, 5, levelManager));


            // Level 2
            Level level2 = new Level("L2", 80f, 10f, Vector2.Zero);
            level2.AddPlatform(new Void(0, -4.75f, level2.Width, 0.5f, levelManager));
            level2.AddPlatform(new Platform(-4.25f, -4, 5, 5));

            level2.AddPlatform(new Platform(4, -3, 2, 1));  
            level2.AddPlatform(new Platform(12, -3f, 4, 1));  
            level2.AddPlatform(new Platform(22, -2f, 4, 1));  
            level2.AddPlatform(new Platform(28, -2.5f, 3, 1));  
            level2.AddPlatform(new Platform(34, -4f, 3, 1));  
            level2.AddPlatform(new Platform(38, -2.5f, 2, 1));  
            level2.AddPlatform(new Platform(44, -3f, 3, 1));  
            level2.AddPlatform(new Platform(50, -3.5f, 3, 1));  
            level2.AddPlatform(new Platform(58, -2f, 4, 1));  
            level2.AddPlatform(new Platform(64, -4f, 3, 1));  
            level2.AddPlatform(new Platform(70, -2.5f, 4, 1));  

            level2.AddPlatform(new FinishPlatform(80, -3, 5, 5, levelManager));
            

            // Level 3
            Level level3 = new Level("L3", 100f, 20f, Vector2.Zero);
            level3.AddPlatform(new Void(0, -4.75f, level3.Width, 0.5f, levelManager));
            level3.AddPlatform(new Platform(-4.25f, -4, 5, 5));

            level3.AddPlatform(new Platform(6, -2.5f, 2, 1));  
            level3.AddPlatform(new Platform(14, -3f, 4, 1));  
            level3.AddPlatform(new Platform(24, -1.5f, 4, 1));  
            level3.AddPlatform(new Platform(30, -2.8f, 3, 1));  
            level3.AddPlatform(new Platform(36, -3.5f, 3, 1));  
            level3.AddPlatform(new Platform(41, -2f, 2, 1));  
            level3.AddPlatform(new Platform(47, -2.3f, 3, 1));  
            level3.AddPlatform(new Platform(53, -3f, 3, 1));  
            level3.AddPlatform(new Platform(60, -1.8f, 4, 1));  
            level3.AddPlatform(new Platform(66, -3.2f, 3, 1));  
            level3.AddPlatform(new Platform(72, -2.5f, 4, 1));  
            level3.AddPlatform(new Platform(78, -2f, 3, 1));  

            level3.AddPlatform(new FinishPlatform(85, -3, 5, 5, levelManager));


            levelManager.AddLevel(level1);
            levelManager.AddLevel(level2);
            levelManager.AddLevel(level3);
            //levelManager.SelectLevel("L1");
            return levelManager;
        }
    }
}