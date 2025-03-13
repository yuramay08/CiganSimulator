using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using System;

class Program
{
    static void Main()
    {
        var settings = new GameWindowSettings();
        var nativeSettings = new NativeWindowSettings()
        {
            Size = new OpenTK.Mathematics.Vector2i(800, 600),   //resoultion of window
            Title = "Cigan Simulator"                           //title
        };

        using (var window = new GameWindow(settings, nativeSettings))
        {
            window.Run();
        }
    }
}