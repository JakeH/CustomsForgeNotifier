using System;
using System.Collections.Generic;

namespace CustomsForgeNotifier
{
    /// <summary>
    /// Builder for entries using the web service in this data handler
    /// </summary>
    internal class EntryBuilderFromJson : ForgeEntry
    {
        public EntryBuilderFromJson(IList<string> data)
        {
            this.SortArtistName = data[0];
            this.ArtistName = data[1];
            this.SongName = data[2];
            this.Album = data[3];
            this.Tuning = GetTuning(data[4]);
            this.AvailableParts = GetParts(data[5]);
            this.HasDynamicDifficulty = !string.Equals("no", data[6], StringComparison.OrdinalIgnoreCase);
            this.AvailablePlatforms = GetPlatforms(data[7]);
            //Rating = 8 --Not sure what purpose this serves
            this.AddedAt = Epoch.AddSeconds(long.Parse(data[9]));
            this.UpdatedAt = Epoch.AddSeconds(long.Parse(data[10]));
            this.Version = data[11];
            this.CreatedByUser = data[12];
            this.DownloadCount = int.Parse(data[13]);
            this.UriToken = data[14];
            this.Id = int.Parse(data[15]);
            this.IsOfficial = !string.Equals("no", data[16], StringComparison.OrdinalIgnoreCase);
            this.DirectPurchaseUri = data[17];
            this.YouTubeUri = string.IsNullOrWhiteSpace(data[18]) ? null : new Uri(data[18]);
            this.InstrumentInfo = GetInstrumentInfo(data[19]);
        }

        /// <summary>
        /// Epoch time, used to convert time retrieved from the web service
        /// </summary>
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected static Tunings GetTuning(string text)
        {
            switch (text)
            {
                case "fsharpstandard": return Tunings.FSharpStandard;
                case "fstandardhigh": return Tunings.FStandardHigh;
                case "estandard": return Tunings.EStandard;
                case "edropd": return Tunings.EDropD;
                case "eflatstandard": return Tunings.EFlatStandard;
                case "eflatdropdflat": return Tunings.EFlatDropDFlat;
                case "dstandard": return Tunings.DStandard;
                case "ddropc": return Tunings.DDropC;
                case "csharpstandard": return Tunings.CSharpStandard;
                case "csharpdropb": return Tunings.CSharpDropB;
                case "cstandard": return Tunings.CStandard;
                case "cdropbflat": return Tunings.CDropBFlat;
                case "bstandard": return Tunings.BStandard;
                case "bflatstandard": return Tunings.BFlatStandard;
                case "bflatdropaflat": return Tunings.BFlatDropAFlat;
                case "bdropa": return Tunings.BDropA;
                case "astandard": return Tunings.AStandard;
                case "aflatstandard": return Tunings.AFlatStandard;
                case "gstandard": return Tunings.GStandard;
                case "gflatstandard": return Tunings.GFlatStandard;
                case "fstandard": return Tunings.FStandard;
                case "octavestandard": return Tunings.OctaveStandard;
                case "opena": return Tunings.OpenA;
                case "openb": return Tunings.OpenB;
                case "openc": return Tunings.OpenC;
                case "opend": return Tunings.OpenD;
                case "opene": return Tunings.OpenE;
                case "openf": return Tunings.OpenF;
                case "openg": return Tunings.OpenG;
                case "celtic": return Tunings.Celtic;

                case "other":
                default:
                    return Tunings.Other;
            }
        }

        protected static InstrumentInfo GetInstrumentInfo(string text)
        {
            InstrumentInfo ret = InstrumentInfo.None;

            foreach (string s in text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (s.Trim().ToLowerInvariant())
                {
                    case "ii_capolead":
                        ret |= InstrumentInfo.CapoLead;
                        break;

                    case "ii_caporhythm":
                        ret |= InstrumentInfo.CapoRhythm;
                        break;

                    case "ii_slidelead":
                        ret |= InstrumentInfo.SlideLead;
                        break;

                    case "ii_sliderhythm":
                        ret |= InstrumentInfo.SlideRhythm;
                        break;

                    case "ii_5stringbass":
                        ret |= InstrumentInfo.FiveStringBass;
                        break;

                    case "ii_6stringbass":
                        ret |= InstrumentInfo.SixStringBass;
                        break;

                    case "ii_7stringguitar":
                        ret |= InstrumentInfo.SevenStringGuitar;
                        break;

                    case "ii_12stringguitar":
                        ret |= InstrumentInfo.TwelveStringGuitar;
                        break;

                    case "ii_heavystrings":
                        ret |= InstrumentInfo.HeavyStrings;
                        break;

                    case "ii_tremolo":
                        ret |= InstrumentInfo.Tremolo;
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Converts a text list of parts into the typed enum
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected static Parts GetParts(string text)
        {
            Parts ret = Parts.None;

            foreach (string s in text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (s.Trim().ToLowerInvariant())
                {
                    case "lead":
                        ret |= Parts.LeadGuitar;
                        break;

                    case "rhythm":
                        ret |= Parts.RhythmGuitar;
                        break;

                    case "bass":
                        ret |= Parts.BassGuitar;
                        break;

                    case "vocals":
                        ret |= Parts.Lyrics;
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Converts a text list of platforms into the typed enum
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected static Platforms GetPlatforms(string text)
        {
            Platforms ret = Platforms.None;

            foreach (string s in text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (s.Trim().ToLowerInvariant())
                {
                    case "pc":
                        ret |= Platforms.PC;
                        break;

                    case "mac":
                        ret |= Platforms.Mac;
                        break;

                    case "ps3":
                        ret |= Platforms.PS3;
                        break;

                    case "xbox360":
                        ret |= Platforms.Xbox360;
                        break;
                }
            }

            return ret;
        }
    }
}