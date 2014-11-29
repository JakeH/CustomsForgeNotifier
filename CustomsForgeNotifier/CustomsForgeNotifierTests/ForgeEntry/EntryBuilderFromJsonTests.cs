using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CustomsForgeNotifier.Tests
{
    [TestClass()]
    public class EntryBuilderFromJsonTests
    {
        /// <summary>
        /// Tests the entire building process regardless of CDLC or official
        /// </summary>
        [TestMethod()]
        public void EntryBuilderFromJsonTest()
        {
            var data = new string[] {
                "The Mars Volta",
                "The Mars Volta",
                "Cicatriz Esp",
                "De-Loused in the Comatorium",
                "estandard",
                ",lead,rhythm,bass,",
                "no",
                ",pc,ps3,",
                "11011",
                "1409276988",
                 "1409277222",
                 "1.0",
                 "RufusDelta",
                 "219",
                 "cicatriz-esp",
                 "7815",
                 "No",
                 "",
                 "http://www.youtube.com/watch?v=UICu_DFpjKA",
                 "ii_heavystrings"};

            var entry = new EntryBuilderFromJson(data);

            Assert.AreEqual("The Mars Volta", entry.SortArtistName);
            Assert.AreEqual("The Mars Volta", entry.ArtistName);
            Assert.AreEqual("Cicatriz Esp", entry.SongName);
            Assert.AreEqual("De-Loused in the Comatorium", entry.Album);
            Assert.AreEqual(Tunings.EStandard, entry.Tuning);
            Assert.AreEqual(Parts.BassGuitar | Parts.RhythmGuitar | Parts.LeadGuitar, entry.AvailableParts);
            Assert.AreEqual(false, entry.HasDynamicDifficulty);
            Assert.AreEqual(Platforms.PC | Platforms.PS3, entry.AvailablePlatforms);

            // skip rating

            Assert.AreEqual(new DateTime(2014, 8, 29, 1, 49, 48, DateTimeKind.Utc), entry.AddedAt);
            Assert.AreEqual(new DateTime(2014, 8, 29, 1, 53, 42, DateTimeKind.Utc), entry.UpdatedAt);
            Assert.AreEqual("1.0", entry.Version);

            Assert.AreEqual("RufusDelta", entry.CreatedByUser);
            Assert.AreEqual(219, entry.DownloadCount);

            Assert.AreEqual("cicatriz-esp", entry.UriToken);

            Assert.AreEqual(7815, entry.Id);
            Assert.AreEqual(false, entry.IsOfficial);
            Assert.AreEqual(InstrumentInfo.HeavyStrings, entry.InstrumentInfo);
            Assert.AreEqual("http://www.youtube.com/watch?v=UICu_DFpjKA", entry.YouTubeUri.ToString());

            Assert.AreEqual("http://customsforge.com/page/customsforge_rs_2014_cdlc.html/_/pc-enabled-rs-2014-cdlc/cicatriz-esp-r7815",
                entry.InformationUri.ToString());

            Assert.AreEqual("http://customsforge.com/process.php?id=7815", entry.DownloadUri.ToString());
        }
    }
}