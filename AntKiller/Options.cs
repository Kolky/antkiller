using System;
using System.Collections.Generic;
using System.Text;
using Mogre;

namespace AntKiller
{
    class Options
    {
        // Start amount 
        public const int antsPerSide = 5;
        public const int foodPieces = 18;

        // Map
        public const int screenX = 3200;
        public const int screenZ = 2400;
        public const int cameraY = 1500;

        // Object
        public static int counter = 1;
        public const int minX = 20;
        public const int maxX = screenX - minX;
        public const int minZ = 20;
        public const int maxZ = screenZ - minZ;

        // Food
        public const int foodMin = 10;
        public const int foodMax = 15;
        public const float foodPerAntNeed = 3;
        public const float foodPerAntUsed = 0.2f;
        public const float secondsPerFood = 30.0f;

        // Ant
        public const float getFoodPercentage = 0.6f;

        public const float returnSpeed = 80.0f;
        public const float searchSpeed = 75.0f;
        public const float walkSpeed = 70.0f;
        public const float carrySpeed = 50.0f;

        public const int health = 30;
        public const double healthRankBonus = 10;
        public const int damagePerRank = 3;

        // SearchState
        public const int pointSpread = 700;

        // Random
        public static Random random = new Random();

        public static int force(int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
        public static Vector3 randomPoint()
        {
            return new Vector3(
                Options.random.Next(minX, maxX),
                0,
                Options.random.Next(minZ, maxZ));
        }
        public static String capital(String str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1).ToLower();
        }
    }
}