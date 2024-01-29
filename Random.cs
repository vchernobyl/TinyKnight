namespace TinyKnight
{
    public static class Random
    {
        private static readonly System.Random random = new System.Random();

        public static int IntValue => random.Next();

        public static double DoubleValue => random.NextDouble();

        public static float FloatValue => (float)random.NextDouble();

        public static int IntRange(int min, int max)
        {
            return random.Next(min, max);
        }

        public static float FloatRange(float min, float max)
        {
            return FloatValue * (max - min) + min;
        }

        public static double DoubleRange(double min, double max)
        {
            return DoubleValue * (max - min) + min;
        }
    }
}
