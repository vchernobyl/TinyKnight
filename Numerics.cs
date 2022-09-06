using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Gravity
{
    public static class Numerics
    {
        public static float VectorToRadians(Vector2 v)
        {
            return MathF.Atan2(v.Y, v.X);
        }

        public static float Approach(float value, float target, float delta)
        {
            return value < target
                ? MathF.Min(value + delta, target)
                : MathF.Max(value - delta, target);
        }

        public static Vector2 Approach(Vector2 value, Vector2 target, float delta)
        {
            var x = Approach(value.X, target.X, delta);
            var y = Approach(value.Y, target.Y, delta);
            return new Vector2(x, y);
        }

        public static Vector2 RadiansToVector(float radians)
        {
            return new Vector2(MathF.Cos(radians), MathF.Sin(radians));
        }

        public static T Pick<T>(int index, T a, T b)
        {
            return index switch
            {
                0 => a,
                1 => b,
                _ => throw new ArgumentException("Index is out of range"),
            };
        }

        public static T Pick<T>(int index, T a, T b, T c)
        {
            return index switch
            {
                0 => a,
                1 => b,
                2 => c,
                _ => throw new ArgumentException("Index is out of range"),
            };
        }

        public static T PickOne<T>(T a, T b)
        {
            return Pick(Random.IntRange(0, 2), a, b);
        }

        public static T PickOne<T>(T a, T b, T c)
        {
            return Pick(Random.IntRange(0, 3), a, b, c);
        }

        public static T PickOne<T>(List<T> list)
        {
            return list[Random.IntRange(0, list.Count)];
        }

        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
