using System;

namespace CustomsForgeNotifier
{
    /// <summary>
    /// Special information regarding the instrument requirements for a Rocksmith song
    /// </summary>
    [Flags]
    public enum InstrumentInfo
    {
        None = 0,
        CapoLead,
        CapoRhythm,
        SlideLead,
        SlideRhythm,
        FiveStringBass,
        SixStringBass,
        SevenStringGuitar,
        TwelveStringGuitar,
        HeavyStrings,
        Tremolo
    }
}