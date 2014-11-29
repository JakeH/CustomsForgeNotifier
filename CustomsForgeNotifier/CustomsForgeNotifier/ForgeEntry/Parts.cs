using System;

namespace CustomsForgeNotifier
{
    /// <summary>
    /// Song parts which are available in a Rocksmith song
    /// </summary>
    [Flags]
    public enum Parts
    {
        None = 0,
        LeadGuitar,
        RhythmGuitar,
        BassGuitar,
        Lyrics
    }
}