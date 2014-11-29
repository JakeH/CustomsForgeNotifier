using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomsForgeNotifier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CustomsForgeNotifier.Tests
{
    [TestClass()]
    public class ForgeEntryTests
    {

        /// <summary>
        /// Tests that the <see cref="ForgeEntry"/> object properly provides Official DLC specific information
        /// </summary>
        [TestMethod()]
        public void OfficialDLCTest()
        {
            ForgeEntry entry = new ForgeEntry
            {
                IsOfficial = true,
                UriToken = "song-2",
                DirectPurchaseUri = "http://www.theriffrepeater.com/rocksmith-2014-setlist/rocksmith-setlist/",
                Id = 8623
            };


            Assert.AreEqual(true, entry.IsOfficial);

            Assert.AreEqual("http://customsforge.com/process.php?id=8623&url=http://www.theriffrepeater.com/rocksmith-2014-setlist/rocksmith-setlist/",
                entry.InformationUri.ToString());

            Assert.AreEqual("http://customsforge.com/process.php?id=8623&url=http://www.theriffrepeater.com/rocksmith-2014-setlist/rocksmith-setlist/",
               entry.DownloadUri.ToString());
           
            
        }

        /// <summary>
        /// Tests that the <see cref="ForgeEntry"/> object properly provides CDLC specific information
        /// </summary>
        [TestMethod()]
        public void CustomDLCTest()
        {
            ForgeEntry entry = new ForgeEntry
            {
                IsOfficial = false,
                UriToken = "cicatriz-esp",
                Id = 7815
            };


            Assert.AreEqual(false, entry.IsOfficial);

            Assert.AreEqual("http://customsforge.com/page/customsforge_rs_2014_cdlc.html/_/pc-enabled-rs-2014-cdlc/cicatriz-esp-r7815",
                entry.InformationUri.ToString());

            Assert.AreEqual("http://customsforge.com/process.php?id=7815", entry.DownloadUri.ToString());
        }
    }
}
