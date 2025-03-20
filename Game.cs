using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace CiganSimulator
{
    public class Game : GameWindow
    {
        private Vector2 playerPosition;
        private Vector2 playerVelocity;
        private float gravity = -9.80665f;
        private bool isGrounded = true;
        private float moveSpeedR = 0f;
        private float moveSpeedL = 0f;
        private float maxSpeed = 20.0f;
        private float moveAcceleration = 0.015f;
        private float jumpForce = 10f;

        private int shaderProgram;
        private int playerVAO;
        private int playerVBO;
        private int positionUniformLocation;
        private int projectionUniformLocation;

        private Matrix4 projection;

        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            { Size = (width, height), Title = title })
        {
            playerPosition = new Vector2(0, 0);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            // Set up orthographic projection matrix (2D)
            projection = Matrix4.CreateOrthographic(10f, 10f, -1f, 1f);

            // Vertex Shader
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec2 aPosition;
                uniform vec2 uPosition;
                uniform mat4 uProjection;
                void main()
                {
                    gl_Position = uProjection * vec4(aPosition + uPosition, 0.0, 1.0);
                }
            ";

            // Fragment Shader
            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
                }
            ";

            // Compile shaders
            int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

            // Create shader program
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Get uniform locations
            positionUniformLocation = GL.GetUniformLocation(shaderProgram, "uPosition");
            projectionUniformLocation = GL.GetUniformLocation(shaderProgram, "uProjection");

            // Define a square using two triangles (instead of deprecated quads)
            float[] vertices =
            {
                -0.5f, -0.5f,  // Bottom-left
                 0.5f, -0.5f,  // Bottom-right
                 0.5f,  0.5f,  // Top-right

                -0.5f, -0.5f,  // Bottom-left
                 0.5f,  0.5f,  // Top-right
                -0.5f,  0.5f   // Top-left
            };

            playerVAO = GL.GenVertexArray();
            playerVBO = GL.GenBuffer();

            GL.BindVertexArray(playerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, playerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var input = KeyboardState;

            // Acceleration
            if (input.IsKeyDown(Keys.Left))
            {
                moveSpeedL += moveAcceleration; // Move left (positive speed)
                if (moveSpeedL > maxSpeed) moveSpeedL = maxSpeed;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                moveSpeedR += moveAcceleration; // Move right
                if (moveSpeedR > maxSpeed) moveSpeedR = maxSpeed;
            }

            // Deceleration (friction)
            if (!input.IsKeyDown(Keys.Left))
            {
                if (moveSpeedL > 0) moveSpeedL -= moveAcceleration*1.5f; // Slow down left movement
                if (moveSpeedL < 0) moveSpeedL = 0; // Stop at zero
            }
            if (!input.IsKeyDown(Keys.Right))
            {
                if (moveSpeedR > 0) moveSpeedR -= moveAcceleration*1.5f; // Slow down right movement
                if (moveSpeedR < 0) moveSpeedR = 0; // Stop at zero
            }

            // Apply movement
            playerPosition.X += (moveSpeedR - moveSpeedL) * (float)args.Time;

            // Jumping
            if (input.IsKeyDown(Keys.Up) && isGrounded)
            {
                playerVelocity.Y = jumpForce;
                isGrounded = false;
            }

            // Apply gravity
            playerVelocity.Y += gravity * (float)args.Time;
            playerPosition += playerVelocity * (float)args.Time;

            // Collision with ground
            if (playerPosition.Y <= -4.5f) // Adjust ground level
            {
                playerPosition.Y = -4.5f;
                playerVelocity.Y = 0;
                isGrounded = true;
            }

            Console.WriteLine("Speed: " + (moveSpeedR - moveSpeedL) + " Left: " + moveSpeedL + " Right: " + moveSpeedR);
        }




        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(shaderProgram);

            // Send uniforms
            GL.Uniform2(positionUniformLocation, playerPosition);
            GL.UniformMatrix4(projectionUniformLocation, false, ref projection);

            GL.BindVertexArray(playerVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6); // Draw as two triangles

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);

            // Adjust the projection matrix dynamically based on the new aspect ratio
            float aspectRatio = e.Width / (float)e.Height;
            projection = Matrix4.CreateOrthographic(10f * aspectRatio, 10f, -1f, 1f);

            // Update projection matrix in the shader
            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(projectionUniformLocation,     false, ref projection);
        }

        private int CompileShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                Console.WriteLine($"Shader Compilation Error ({type}): {GL.GetShaderInfoLog(shader)}");
            }
            return shader;
        }
    }
}
