using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace CiganSimulator
{
    class Program
    {
        [Obsolete]
        static void Main()
        {
            using (Game game = new Game(800, 600, "Kradnut Zelezo", "L1")) //res of game window
            {
                game.Run();
                
            }
        }
    }
}