using System.Numerics;

namespace CiganSimulator
{
    public class Platform
    {
        public int x;
        public int y;
        public int width;
        public int height;
        private Vector2 mapVelocity;

        public Platform(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            mapVelocity = Vector2.Zero;
        }

        public Vector2 GetMapVelocity()
        {
            return mapVelocity;
        }
    }
}