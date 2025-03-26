using System.Numerics;

namespace CiganSimulator
{
    public class Platform
    {
        public float x;
        public float y;
        public float width;
        public float height;
        private Vector2 mapVelocity;

        public Platform(float x, float y, float width, float height)
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
        public bool IsCollidingWithPlayer(Vector2 playerPos, Vector2 playerSize)
        {
            float halfPlayerW = playerSize.X * 0.5f;
            float halfPlayerH = playerSize.Y * 0.5f;

            float halfPlatformW = width * 0.5f;
            float halfPlatformH = height * 0.5f;

            // Check horizontal overlap
            bool xOverlap = (playerPos.X + halfPlayerW > x - halfPlatformW) &&
                            (playerPos.X - halfPlayerW < x + halfPlatformW);

            // Check vertical overlap
            bool yOverlap = (playerPos.Y + halfPlayerH > y - halfPlatformH) &&
                            (playerPos.Y - halfPlayerH < y + halfPlatformH);

            return xOverlap && yOverlap;
        }
        public bool IsCollidingWithPlayerOnTop(Vector2 playerPos, Vector2 playerSize, ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            //use after IsCollidingWithPlayer return true
            float halfPlayerW = playerSize.X * 0.5f;
            float halfPlayerH = playerSize.Y * 0.5f;
            if(playerPos.Y - halfPlayerH >= y - height * 0.25f)//reserve pre rychle padanie
            {
                playerPosition.Y = y + height * 0.5f + halfPlayerH;
                return true;
            }
            return false;
        }
        public bool IsCollidingWithPlayerFromBottom(Vector2 playerPos, Vector2 playerSize,  ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            //use after IsCollidingWithPlayer return true
            float halfPlayerW = playerSize.X * 0.5f;
            float halfPlayerH = playerSize.Y * 0.5f;
            if(playerPos.Y + halfPlayerH >= y - height * 0.48f)
            {
                playerPosition.Y = y - height * 0.5f - halfPlayerH;
                return true;
            }
            return false;
        }
        public bool IsCollidingWithPlayerFromSide(Vector2 playerPos, Vector2 playerSize,  ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            //use after IsCollidingWithPlayer return true
            float halfPlayerW = playerSize.X * 0.5f;
            float halfPlayerH = playerSize.Y * 0.5f;
            if(playerPos.X - halfPlayerW >= x + width * 0.48f)//reserve pre rychly pohyb
            {

                playerPosition.X = x + width * 0.5f + halfPlayerH;
                return true;
            }
            else if(playerPos.X + halfPlayerW <= x - width * 0.48f)//reserve pre rychly pohyb
            {
                playerPosition.X = x - width * 0.5f - halfPlayerH;
                return true;
            }
            return false;
        }
    }
}