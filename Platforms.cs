using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace CiganSimulator
{
    public class Platform
    {
        public float x;
        public float y;
        public float width;
        public float height;
        private Vector2 mapVelocity;
        public int textureID { get; set; }

        public Platform(float x, float y, float width, float height, int textureID = -1)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.textureID = textureID;
            mapVelocity = Vector2.Zero;
        }

        public Vector2 GetMapVelocity()
        {
            return mapVelocity;
        }

        public void SetTexture(int textureID)
        {
            this.textureID = textureID;
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

        public virtual bool IsCollidingWithPlayerOnTop(Vector2 playerPos, Vector2 playerSize, ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            float halfPlayerH = playerSize.Y * 0.5f;
            if (playerPos.Y - halfPlayerH >= y - height * 0.25f)
            {
                playerPosition.Y = y + height * 0.5f + halfPlayerH;
                return true;
            }
            return false;
        }
        public virtual bool IsCollidingWithPlayerOnTop(Vector2 playerPos, Vector2 playerSize, ref OpenTK.Mathematics.Vector2 playerPosition, bool edge)
        {
            float halfPlayerH = playerSize.Y * 0.5f;
            if (playerPos.Y - halfPlayerH >= y - height * 0.45f)
            {
                // playerPosition.Y = y + height * 0.5f + halfPlayerH;
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
        public bool IsCollidingWithPlayerFromSide(Vector2 playerPos, Vector2 playerSize,  ref OpenTK.Mathematics.Vector2 playerPosition, bool edge)
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

    public class FinishPlatform : Platform
    {
        private LevelManager levelManager;

        public FinishPlatform(float x, float y, float width, float height, LevelManager levelManager) : base(x, y, width, height)
        {
            this.levelManager = levelManager;
        }

        public override bool IsCollidingWithPlayerOnTop(Vector2 playerPos, Vector2 playerSize, ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            if (base.IsCollidingWithPlayerOnTop(playerPos, playerSize, ref playerPosition))
            {
                Console.WriteLine("Level Complete!");
                levelManager.GoToNextLevel(ref playerPosition);
                return true;
            }
            return false;
        }
    }

    public class Void : Platform
    {
        private LevelManager levelManager;

        public Void(float x, float y, float width, float height, LevelManager levelManager) : base(x, y, width, height)
        {
            this.levelManager = levelManager;
        }

        public override bool IsCollidingWithPlayerOnTop(Vector2 playerPos, Vector2 playerSize, ref OpenTK.Mathematics.Vector2 playerPosition)
        {
            if (base.IsCollidingWithPlayerOnTop(playerPos, playerSize, ref playerPosition))
            {
                levelManager.RestartLevel(ref playerPosition);
                return true;
            }
            return false;
        }
    }
}