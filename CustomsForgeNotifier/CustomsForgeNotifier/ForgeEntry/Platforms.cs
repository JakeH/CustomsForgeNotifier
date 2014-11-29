using System;

namespace CustomsForgeNotifier
{
    /// <summary>
    /// Hardware platforms which Rocksmith runs on
    /// </summary>
    [Flags]
    public enum Platforms
    {
        None = 0,
        PC,
        PS3,
        Xbox360,
        Mac
    }
}