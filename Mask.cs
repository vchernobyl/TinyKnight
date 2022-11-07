using System;

namespace Gravity
{
    [Flags]
    public enum Mask
    {
        Default = 0,
        Level = 1,
        Player = 2,
        Enemy = 4,
        Item = 8,
        All = 255,
    }
}
