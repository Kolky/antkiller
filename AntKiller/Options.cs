using System;
using System.Collections.Generic;
using System.Text;
using Mogre;

namespace AntKiller
{
    class Options
    {
        // Start amount 
        public const int antsPerSide = 10;
        public const int foodPieces = 20;

        // Object
        public static int counter = 1;
        public const float minX = 20f;
        public const float maxX = 1480f;
        public const float minZ = 20f;
        public const float maxZ = 1480f;

        // Food
        public const int foodMin = 10;
        public const int foodMax = 15;
        public const int foodPerAntNeed = 3;
        public const int foodPerAntUsed = 1;
        public const float secondsPerFood = 30.0f;

        // Ant
        public const float getFoodPercentage = 0.6f;
        public const float walkSpeed = 70.0f;
        public const int health = 30;
        public const double healthRankBonus = 10;

        // SearchState
        public const int pointSpread = 500;

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
            return new Vector3(Options.random.Next(20, 1480), 0, Options.random.Next(20, 1480));
        }
        public static String capital(String str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1).ToLower();
        }
    }
}