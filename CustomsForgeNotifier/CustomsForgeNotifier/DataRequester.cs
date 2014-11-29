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
                    ForgeEntry entry = new EntryBuilderFromJson(datum);

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
    }
}