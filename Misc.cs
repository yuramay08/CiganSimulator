using System;
using OpenTKVector2 = OpenTK.Mathematics.Vector2;
using SystemNumericsVector2 = System.Numerics.Vector2;

namespace CiganSimulator
{
    public static class Misc
    {
        public static OpenTKVector2 ToOpenTK(this SystemNumericsVector2 source)
        {
            return new OpenTKVector2(source.X, source.Y);
        }

        public static SystemNumericsVector2 ToSystemNumerics(this OpenTKVector2 source)
        {
            return new SystemNumericsVector2(source.X, source.Y);
        }
    }
}