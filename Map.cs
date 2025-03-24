using System;
using System.Collections.Generic;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace CiganSimulator
{
    // In Map.cs, add a reference to the shader program and uniform location:
    public class Map
    {
        private List<Platform> platforms;
        private Vector2 mapOffset;
        private int shaderProgram;
        private int positionUniformLocation;

        public Map(int shaderProgram, int positionUniformLocation)
        {
            platforms = new List<Platform>();
            mapOffset = Vector2.Zero;
            this.shaderProgram = shaderProgram;
            this.positionUniformLocation = positionUniformLocation;
        }

        public void AddPlatform(Platform platform)
        {
            platforms.Add(platform);
        }

        public void Update(Vector2 playerPosition, Vector2 playerVelocity)
        {
            if (playerPosition.X > 5f)
            {
                mapOffset.X -= playerVelocity.X * 0.1f;
            }
        }

        public void Render(int vao)
        {
            // Reuse the same rectangle geometry
            foreach (var platform in platforms)
            {
                // Position = platform coords + map offset
                var platformPosition = new OpenTK.Mathematics.Vector2(
                    platform.x + (int)mapOffset.X,
                    platform.y
                );

                GL.Uniform2(positionUniformLocation, platformPosition);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }
    }
}