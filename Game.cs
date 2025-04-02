using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;

namespace CiganSimulator
{
    public class Game : GameWindow
    {
        private string initialLevel;
        
        private Vector2 playerPositioNOnScreen;// 0,0 for now
        private Vector2 playerPosition; //world position
        private Vector2 playerVelocity;
        private float gravity = -9.80665f * 1.2f;
        public bool isGrounded = false;
        private float moveSpeedR = 0f;
        private float moveSpeedL = 0f;
        private float maxSpeed = 10.0f;
        private float moveAcceleration = 0.015f;
        private float jumpForce = 6.5f;
        private LevelManager levelManager;
        private Map map;
        private Vector2 cameraPosition = new Vector2(0, 0f);
        private float visibleHeight = 10f;
        private float visibleWidth;
        private int shaderProgram;
        private int playerVAO;
        private int playerVBO;
        private int positionUniformLocation;
        private int projectionUniformLocation;

        private Matrix4 projection;

        [Obsolete]
        public Game(int width, int height, string title, string startLevel)
        : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (width, height),
            Title = title,
            WindowBorder = WindowBorder.Fixed, // Disable resizing
            Flags = ContextFlags.Default
        })
        {
            playerPosition = new Vector2(-6.0f + -1.0f / 6.0f, -4.5f);//down left side of the screen if 800*600
            initialLevel = startLevel;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            
            // Orthographic projection
            float aspectRatio = Size.X / (float)Size.Y;
            projection = Matrix4.CreateOrthographic(10f * aspectRatio, 10f, -1f, 1f);

            // Prepare shader
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec2 aPosition;
                uniform vec2 uPosition;
                uniform vec2 uScale;
                uniform mat4 uProjection;
                void main()
                {
                    gl_Position = uProjection * vec4(aPosition * uScale + uPosition, 0.0, 1.0);
                }
            ";
            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(1.0, 0.0, 0.0, 1.0);//dos colores
                }
            ";

            int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            positionUniformLocation = GL.GetUniformLocation(shaderProgram, "uPosition");
            projectionUniformLocation = GL.GetUniformLocation(shaderProgram, "uProjection");
            int scaleUniformLocation = GL.GetUniformLocation(shaderProgram, "uScale");

            // Define quad vertices for rendering
            float[] vertices =
            {
                //v komentaroch je ako by to vyzeralo v normalnom svete ale je to cigansky kod pre cigan simulator
                -0.5f, -0.5f,// 0,0
                0.5f, -0.5f,// 1,0
                0.5f,  0.5f,//1,1

                -0.5f, -0.5f,//0,0
                0.5f,  0.5f,//1,1
                -0.5f,  0.5f//0,1
            };

            playerVAO = GL.GenVertexArray();
            playerVBO = GL.GenBuffer();
            GL.BindVertexArray(playerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, playerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            // Instantiate level manager and levels
            levelManager = LevelSetup.CreateLevelManager();
            levelManager.SelectLevel(initialLevel, ref playerPosition);

            // Simple map for demonstration
            map = new Map(shaderProgram, positionUniformLocation);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var input = KeyboardState;

            // Movement
            if (input.IsKeyDown(Keys.Left))
            {
                moveSpeedL += moveAcceleration;
                if (moveSpeedL > maxSpeed) moveSpeedL = maxSpeed;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                moveSpeedR += moveAcceleration;
                if (moveSpeedR > maxSpeed) moveSpeedR = maxSpeed;
            }
            if (!input.IsKeyDown(Keys.Left))
            {
                moveSpeedL -= moveAcceleration * 1f;
                if (moveSpeedL < 0) moveSpeedL = 0;
            }
            if (!input.IsKeyDown(Keys.Right))
            {
                moveSpeedR -= moveAcceleration * 1f;
                if (moveSpeedR < 0) moveSpeedR = 0;
            }

            isGrounded = false;
            
            //goofy collision for every platform
            foreach (var platform in levelManager.CurrentLevel.Platforms)
            {
                // Convert player position and size to System.Numerics.Vector2 before calling the collision check
                OpenTK.Mathematics.Vector2 playerSize = new OpenTK.Mathematics.Vector2(1.0f, 1.0f);
                if (platform.IsCollidingWithPlayer(playerPosition.ToSystemNumerics(), playerSize.ToSystemNumerics()))
                {
                    if(platform.IsCollidingWithPlayerFromSide(playerPosition.ToSystemNumerics(), playerSize.ToSystemNumerics(), ref playerPosition))
                    {
                        playerVelocity.X = 0;
                        moveSpeedL = 0;
                        moveSpeedR = 0;
                        Debug.WriteLine("Side collision");
                    }
                    else if(platform.IsCollidingWithPlayerOnTop(playerPosition.ToSystemNumerics(), playerSize.ToSystemNumerics(), ref playerPosition))
                    {
                        isGrounded = true;
                        playerVelocity.Y = 0;
                        
                    }
                    else if(platform.IsCollidingWithPlayerFromBottom(playerPosition.ToSystemNumerics(), playerSize.ToSystemNumerics(), ref playerPosition))
                    {
                        playerVelocity.Y = 0;
                    }
                }
            }
            // Ground collision
            if (playerPosition.Y <= -4.5f)
            {
                playerPosition.Y = -4.5f;
                playerVelocity.Y = 0;
                isGrounded = true;
            }
            // Jump
            if (input.IsKeyDown(Keys.Up) && isGrounded)
            {   
                playerVelocity.Y = jumpForce;
                isGrounded = false;
            }

            // Apply horizontal movement
            playerPosition.X += (moveSpeedR - moveSpeedL) * (float)args.Time;
            // Gravity
            
            playerPosition += playerVelocity * (float)args.Time;
            playerVelocity.Y += gravity * (float)args.Time;

            

            // Switch levels with F1/F2/F3 for demo
            if (input.IsKeyPressed(Keys.F1))
            {
                levelManager.SelectLevel("L1", ref playerPosition);
            }
            if (input.IsKeyPressed(Keys.F2))
            {
                levelManager.SelectLevel("L2", ref playerPosition);
            }
            if (input.IsKeyPressed(Keys.F3)) {
                levelManager.SelectLevel("L3", ref playerPosition);
            }
            
            // Update map
            map.Update(Misc.ToSystemNumerics(playerPosition), Misc.ToSystemNumerics(playerVelocity));
            Debug.WriteLine($"Player position: {playerPosition}");
            // cameraPosition = playerPosition;
            UpdateCameraPosition(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(projectionUniformLocation, false, ref projection);

            // Render player (subtract cameraPosition)
            GL.Uniform2(positionUniformLocation, playerPosition - cameraPosition);
            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "uScale"), new Vector2(1.0f, 1.0f));
            GL.BindVertexArray(playerVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            // Render current level’s platforms (subtract cameraPosition)
            int scaleUniformLocation = GL.GetUniformLocation(shaderProgram, "uScale");
            foreach (var platform in levelManager.CurrentLevel.Platforms)
            {
                var platformPos = new Vector2(platform.x, platform.y) - cameraPosition;
                GL.Uniform2(positionUniformLocation, platformPos);
                GL.Uniform2(scaleUniformLocation, new Vector2(platform.width, platform.height));
                
                GL.BindVertexArray(playerVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            // Render map (if applicable) – subtract cameraPosition if needed in map.Render
            map.Render(playerVAO);

            SwapBuffers();
        }


        private void UpdateCameraPosition(FrameEventArgs args)
        {
            // cameraPosition = playerPosition;
            float lerpSpeed = 2f;
            cameraPosition += (playerPosition - cameraPosition) * lerpSpeed * (float)args.Time;
            if(cameraPosition.X < 0.0f)
            {
                cameraPosition.X = 0.0f;
            }
            if(cameraPosition.X > levelManager.CurrentLevel.Width)
            {
                cameraPosition.X = levelManager.CurrentLevel.Width;
            }

            if(cameraPosition.Y < 0.0f)
            {
                cameraPosition.Y = 0.0f;
            }
            if(cameraPosition.Y > levelManager.CurrentLevel.Height)
            {
                cameraPosition.Y = levelManager.CurrentLevel.Height;
            }
            
            {
                //YES
            }


            Debug.WriteLine($"Camera position: {cameraPosition}");
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);

            float aspectRatio = e.Width / (float)e.Height;
            projection = Matrix4.CreateOrthographic(10f * aspectRatio, 10f, -1f, 1f);

            GL.UseProgram(shaderProgram);
            GL.UniformMatrix4(projectionUniformLocation, false, ref projection);
        }

        private int CompileShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                Console.WriteLine(GL.GetShaderInfoLog(shader));
            }
            return shader;
        }
    }
}