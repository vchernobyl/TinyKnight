using System;

namespace Gravity
{
    [Flags]
    public enum Mask
    {
        Default = 0,
        None = 1,
        Level = 2,
        Player = 4,
        Enemy = 8,
        Item = 16,
        All = 255,
    }
}
