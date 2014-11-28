namespace CustomsForgeNotifier
{
    public enum Tunings
    {
        fsharpstandard,
        fstandardhigh,
        estandard,
        edropd,
        eflatstandard,
        eflatdropdflat,
        dstandard,
        ddropc,
        csharpstandard,
        csharpdropb,
        cstandard,
        cdropbflat,
        bstandard,
        bflatstandard,
        bflatdropaflat,
        bdropa,
        astandard,
        aflatstandard,
        gstandard,
        gflatstandard,
        fstandard,
        octavestandard,
        opena,
        openb,
        openc,
        opend,
        opene,
        openf,
        openg,
        celtic,
        other
    }

    public static class TuningExtensions
    {
        public static string ToFriendlyString(this Tunings tuning)
        {
            switch (tuning)
            {
                case Tunings.fsharpstandard: return "F# Standard (High)";
                case Tunings.fstandardhigh: return "F Standard (High)";
                case Tunings.estandard: return "E Standard";
                case Tunings.edropd: return "Drop D";
                case Tunings.eflatstandard: return "Eb Standard";
                case Tunings.eflatdropdflat: return "Eb Drop Db";
                case Tunings.dstandard: return "D Standard";
                case Tunings.ddropc: return "D Drop C";
                case Tunings.csharpstandard: return "C# Standard";
                case Tunings.csharpdropb: return "C# Drop B";
                case Tunings.cstandard: return "C Standard";
                case Tunings.cdropbflat: return "C Drop Bb";
                case Tunings.bstandard: return "B Standard";
                case Tunings.bflatstandard: return "Bb Standard";
                case Tunings.bflatdropaflat: return "Bb Drop Ab";
                case Tunings.bdropa: return "B Drop A";
                case Tunings.astandard: return "A Standard";
                case Tunings.aflatstandard: return "Ab Standard";
                case Tunings.gstandard: return "G Standard";
                case Tunings.gflatstandard: return "Gb Standard (Low)";
                case Tunings.fstandard: return "F Standard (Low)";
                case Tunings.octavestandard: return "Octave (drop 1 octave)";
                case Tunings.opena: return "Open A";
                case Tunings.openb: return "Open B";
                case Tunings.openc: return "Open C";
                case Tunings.opend: return "Open D";
                case Tunings.opene: return "Open E";
                case Tunings.openf: return "Open F";
                case Tunings.openg: return "Open G";
                case Tunings.celtic: return "Celtic (D A D G A D)";
                case Tunings.other:
                default: return "Other";
            }
        }
    }
}