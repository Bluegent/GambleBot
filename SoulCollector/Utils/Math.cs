using System;

namespace SoulCollector.Utils
{
    public class Math
    {
        private static Random _gen = new Random();
        public static int Clamp(int value, int min, int max)
        {
            if (value >= max)
                return max;
            if (value <= min)
                return min;
            return value;
        }

        public static int Random(int min, int max)
        {
            return min + _gen.Next(max - min);
        }
    }
}