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

        //goofy collision
        public bool IsCollidingWithPlayer(Vector2 playerPos, Vector2 playerSize, ref OpenTK.Mathematics.Vector2 playerVelocity)
        {
            // Check for collision in the horizontal (X) and vertical (Y) axis
            if (playerPos.X + playerSize.X > x && playerPos.X < x + width) // X overlap
            {
                if (playerPos.Y + playerSize.Y > y && playerPos.Y + playerSize.Y <= y + height) // Y overlap (falling)
                {
                    // Player collides with platform from above (falling)
                    playerPos.Y = y - playerSize.Y; // Adjust player position to be on top of the platform
                    playerVelocity.Y = 0; // Stop vertical downward movement
                    return true;
                }
            }
            return false;
        }
    }
}