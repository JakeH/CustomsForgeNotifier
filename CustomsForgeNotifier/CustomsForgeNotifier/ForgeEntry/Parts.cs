using System;

namespace CustomsForgeNotifier
{
    [Flags]
    public enum Parts
    {
        None = 0,
        LeadGuitar = 1,
        RhythmGuitar = 2,
        BassGuitar = 4,
        Lyrics = 8
    }
}