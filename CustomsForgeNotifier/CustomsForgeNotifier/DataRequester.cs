using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace CustomsForgeNotifier
{
    public static class DataRequester
    {
        private static readonly ILog Logger = LogHelper.GetLog();

        /// <summary>
        /// Gets the appropriate Uri for the web service
        /// </summary>
        /// <param name="displayStart">The index from which to start returning records</param>
        /// <param name="displayLength">The number of records to return</param>
        /// <returns></returns>
        private static Uri GetUri(int displayStart, int displayLength)
        {
            //TODO: this uri is messy, see if some of the parameters can be removed
            return new Uri(
                string.Format("http://search.customsforge.com/serverside.php?sEcho=1&iColumns=16&sColumns=&iDisplayStart={0}&iDisplayLength={1}&mDataProp_0=0&mDataProp_1=1&mDataProp_2=2&mDataProp_3=3&mDataProp_4=4&mDataProp_5=5&mDataProp_6=6&mDataProp_7=7&mDataProp_8=8&mDataProp_9=9&mDataProp_10=10&mDataProp_11=11&mDataProp_12=12&mDataProp_13=13&mDataProp_14=14&mDataProp_15=15&sSearch=&bRegex=false&sSearch_0=&bRegex_0=false&bSearchable_0=true&sSearch_1=&bRegex_1=true&bSearchable_1=true&sSearch_2=&bRegex_2=false&bSearchable_2=true&sSearch_3=&bRegex_3=false&bSearchable_3=true&sSearch_4=&bRegex_4=false&bSearchable_4=true&sSearch_5=&bRegex_5=true&bSearchable_5=true&sSearch_6=&bRegex_6=true&bSearchable_6=true&sSearch_7=&bRegex_7=true&bSearchable_7=true&sSearch_8=&bRegex_8=false&bSearchable_8=true&sSearch_9=&bRegex_9=false&bSearchable_9=true&sSearch_10=&bRegex_10=false&bSearchable_10=true&sSearch_11=&bRegex_11=false&bSearchable_11=true&sSearch_12=&bRegex_12=false&bSearchable_12=true&sSearch_13=&bRegex_13=false&bSearchable_13=true&sSearch_14=&bRegex_14=false&bSearchable_14=true&sSearch_15=&bRegex_15=false&bSearchable_15=true&iSortCol_0=10&sSortDir_0=desc&iSortingCols=1&bSortable_0=false&bSortable_1=true&bSortable_2=true&bSortable_3=true&bSortable_4=true&bSortable_5=true&bSortable_6=true&bSortable_7=true&bSortable_8=true&bSortable_9=true&bSortable_10=true&bSortable_11=true&bSortable_12=true&bSortable_13=true&bSortable_14=true&bSortable_15=true",
                displayStart, displayLength));
        }

        /// <summary>
        /// Gets new entries from the web service
        /// </summary>
        /// <param name="lastUpdatedTime">Entries updated after this time will be returned</param>
        /// <param name="retrievalLimit">No more than this amount of entries will be returned</param>
        /// <returns></returns>
        public static IEnumerable<ForgeEntry> GetForgeEntries(DateTime lastUpdatedTime, int retrievalLimit)
        {
            int retrievedCount = 0;
            bool keepProcessing = true;

            while (keepProcessing && retrievedCount < retrievalLimit)
            {
                // rate limit this a bit
                if (retrievedCount > 0)
                    Thread.Sleep(5000);

                RequestDataStructure response = null;

                try
                {
                    response = GetRemoteData(GetUri(retrievedCount, 25));
                }
                catch (Exception ex)
                {
                    Logger.Error("Could not get data", ex);
                    yield break;
                }

                foreach (var datum in response.Data)
                {
                    ForgeEntry entry = new EntryBuilder(datum);

                    if (entry.UpdatedAt > lastUpdatedTime)
                    {
                        retrievedCount++;
                        yield return entry;
                    }
                    else
                    {
                        keepProcessing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the retrieval of data and conversion into <see cref="RequestDataStructure"/> from the web service
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static RequestDataStructure GetRemoteData(Uri uri)
        {
            RequestDataStructure data = null;

            using (WebClient client = new WebClient())
            using (StreamReader reader = new StreamReader(client.OpenRead(uri)))
            {
                data = JsonConvert.DeserializeObject<RequestDataStructure>(reader.ReadToEnd());
            }
            return data;
        }

        private sealed class RequestDataStructure
        {
            [JsonProperty("sEcho")]
            public int Echo = 0;

            [JsonProperty("iTotalRecords")]
            public int TotalRecords = 0;

            [JsonProperty("iTotalDisplayRecords")]
            public int TotalDisplayRecords = 0;

            [JsonProperty("aaData")]
            public List<List<string>> Data = null;
        }

        /// <summary>
        /// Builder for entries using the web service in this data handler
        /// </summary>
        private sealed class EntryBuilder : ForgeEntry
        {
            public EntryBuilder(IList<string> data)
            {
                this.SortArtistName = data[0];
                this.ArtistName = data[1];
                this.SongName = data[2];
                this.Album = data[3];
                this.Tuning = (Tunings)Enum.Parse(typeof(Tunings), data[4]);
                this.AvailableParts = GetParts(data[5]);
                this.HasDynamicDifficulty = !string.Equals("no", data[6], StringComparison.OrdinalIgnoreCase);
                this.AvailablePlatforms = GetPlatforms(data[7]);
                //Rating = 8 --Not sure what purpose this serves
                this.AddedAt = Epoch.AddSeconds(long.Parse(data[9]));
                this.UpdatedAt = Epoch.AddSeconds(long.Parse(data[10]));
                this.Version = data[11];
                this.CreatedByUser = data[12];
                this.DownloadCount = int.Parse(data[13]);
                this.Furl = data[14];
                this.Id = int.Parse(data[15]);
                this.IsOfficial = !string.Equals("no", data[16], StringComparison.OrdinalIgnoreCase);
                this.Direct = data[17];
                this.YouTubeUri = string.IsNullOrWhiteSpace(data[18]) ? null : new Uri(data[18]);
                this.InstrumentInfo = GetInstrumentInfo(data[19]);
            }

            /// <summary>
            /// Epoch time, used to convert time retrieved from the web service
            /// </summary>
            private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            private static InstrumentInfo GetInstrumentInfo(string text)
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
            private static Parts GetParts(string text)
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
            private static Platforms GetPlatforms(string text)
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
}