using System;

namespace CustomsForgeNotifier
{
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