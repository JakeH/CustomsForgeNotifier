using System;

namespace CustomsForgeNotifier
{
    [Flags]
    public enum Platforms
    {
        None = 0,
        PC = 1,
        PS3 = 2,
        Xbox360 = 4,
        Mac = 8
    }
}