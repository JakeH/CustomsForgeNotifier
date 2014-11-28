using System;
using System.Collections.Generic;
using System.Text;

namespace CustomsForgeNotifier
{
    /// <summary>
    /// Represents an entry from the Customs Forge web service
    /// </summary>
    public class ForgeEntry
    {
        /// <summary>
        /// Id of the entry
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// Artist name, in a format suitable for sorting
        /// </summary>
        public string SortArtistName { get; protected set; }

        /// <summary>
        /// Artist name
        /// </summary>
        public string ArtistName { get; protected set; }

        /// <summary>
        /// Song name
        /// </summary>
        public string SongName { get; protected set; }

        /// <summary>
        /// Album name
        /// </summary>
        public string Album { get; protected set; }

        /// <summary>
        /// Tuning of the song
        /// </summary>
        public Tunings Tuning { get; protected set; }

        /// <summary>
        /// Available parts in entry (lead, bass, etc)
        /// </summary>
        public Parts AvailableParts { get; protected set; }

        /// <summary>
        /// Available platforms this entry is compatible with (PC, Xbox 360, etc)
        /// </summary>
        public Platforms AvailablePlatforms { get; protected set; }

        /// <summary>
        /// Dynamic Difficulty is available for this entry
        /// </summary>
        public bool HasDynamicDifficulty { get; protected set; }

        /// <summary>
        /// When the entry was originally added to Customs Forge
        /// </summary>
        public DateTime AddedAt { get; protected set; }

        /// <summary>
        /// When the entry was last updated
        /// </summary>
        public DateTime UpdatedAt { get; protected set; }

        /// <summary>
        /// Version of this entry
        /// </summary>
        public string Version { get; protected set; }

        /// <summary>
        /// Download count
        /// </summary>
        public int DownloadCount { get; protected set; }

        /// <summary>
        /// True if the entry an official (from Rocksmith publisher) entry as opposed to a fan-made entry
        /// </summary>
        public bool IsOfficial { get; protected set; }

        /// <summary>
        /// Link to YouTube video
        /// </summary>
        public Uri YouTubeUri { get; protected set; }

        /// <summary>
        /// Name of the user who created this entry
        /// </summary>
        public string CreatedByUser { get; protected set; }

        /// <summary>
        /// Special instrument requirements
        /// </summary>
        public InstrumentInfo InstrumentInfo { get; protected set; }


        /// <summary>
        /// Unknown
        /// </summary>
        protected string Direct;

        /// <summary>
        /// Uri token
        /// </summary>
        protected string Furl;

        /// <summary>
        /// Uri to the Customs Forge information page for this entry
        /// </summary>
        public Uri InformationUri
        {
            get
            {
                if (this.IsOfficial)
                    return new Uri(string.Format("http://customsforge.com/process.php?id={0}&url={1}",
                        this.Id, this.Direct));
                else
                    return new Uri(string.Format("http://customsforge.com/page/customsforge_rs_2014_cdlc.html/_/pc-enabled-rs-2014-cdlc/{0}-r{1}",
                        this.Furl, this.Id));
            }
        }

        /// <summary>
        /// Direct link to the download service for this entry
        /// </summary>
        public Uri DownloadUri
        {
            get
            {
                //TODO: probably not necessary when IsOfficial
                return new Uri(string.Format("http://customsforge.com/process.php?id={0}", this.Id));
            }
        }



        /// <summary>
        /// Returns the explanation of the tuning
        /// </summary>
        /// <returns></returns>
        public string TuningExplained()
        {
            switch (this.Tuning)
            {
                case Tunings.FSharpStandard: return "F# Standard (High)";
                case Tunings.FStandardHigh: return "F Standard (High)";
                case Tunings.EStandard: return "E Standard";
                case Tunings.EDropD: return "Drop D";
                case Tunings.EFlatStandard: return "Eb Standard";
                case Tunings.EFlatDropDFlat: return "Eb Drop Db";
                case Tunings.DStandard: return "D Standard";
                case Tunings.DDropC: return "D Drop C";
                case Tunings.CSharpStandard: return "C# Standard";
                case Tunings.CSharpDropB: return "C# Drop B";
                case Tunings.CStandard: return "C Standard";
                case Tunings.CDropBFlat: return "C Drop Bb";
                case Tunings.BStandard: return "B Standard";
                case Tunings.BFlatStandard: return "Bb Standard";
                case Tunings.BFlatDropAFlat: return "Bb Drop Ab";
                case Tunings.BDropA: return "B Drop A";
                case Tunings.AStandard: return "A Standard";
                case Tunings.AFlatStandard: return "Ab Standard";
                case Tunings.GStandard: return "G Standard";
                case Tunings.GFlatStandard: return "Gb Standard (Low)";
                case Tunings.FStandard: return "F Standard (Low)";
                case Tunings.OctaveStandard: return "Octave (drop 1 octave)";
                case Tunings.OpenA: return "Open A";
                case Tunings.OpenB: return "Open B";
                case Tunings.OpenC: return "Open C";
                case Tunings.OpenD: return "Open D";
                case Tunings.OpenE: return "Open E";
                case Tunings.OpenF: return "Open F";
                case Tunings.OpenG: return "Open G";
                case Tunings.Celtic: return "Celtic (D A D G A D)";
                case Tunings.Other:
                default: return "Other";
            }
        }

        /// <summary>
        /// Returns explanation 
        /// </summary>
        /// <returns></returns>
        public List<string> InstrumentInfoExplained()
        {
            List<string> ret = new List<string>();

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.CapoLead))
                ret.Add("Capo on Lead guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.CapoRhythm))
                ret.Add("Capo on Rhythm guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.SlideLead))
                ret.Add("Slide on Lead guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.SlideRhythm))
                ret.Add("Slide on Rhythm guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.FiveStringBass))
                ret.Add("5-string bass guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.SixStringBass))
                ret.Add("6-string bass guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.SevenStringGuitar))
                ret.Add("7-string guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.TwelveStringGuitar))
                ret.Add("12-string guitar required");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.HeavyStrings))
                ret.Add("Heavy-gauge strings recommended");

            if (this.InstrumentInfo.HasFlag(InstrumentInfo.Tremolo))
                ret.Add("Tremolo recommended");

            return ret;

        }


    }
}