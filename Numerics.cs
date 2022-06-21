using System;

namespace Gravity
{
    public static class Numerics
    {
        public static T Pick<T>(int index, T a, T b)
        {
            return index switch
            {
                0 => a,
                1 => b,
                _ => throw new ArgumentException("Index is out of range"),
            };
        }

        public static T PickOne<T>(T a, T b)
        {
            return Pick(Random.IntRange(0, 2), a, b);
        }
    }
}
