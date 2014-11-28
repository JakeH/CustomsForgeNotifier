using System;

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
    }
}