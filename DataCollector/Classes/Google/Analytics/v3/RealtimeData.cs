using System.Collections.Generic;

namespace DataCollector.Analytics.v3
{
    class RealtimeData
    {
        /// <summary>
        /// Extracts the data of a Google Realtime request.
        /// </summary>
        /// <param name="rt">The RealtimeData response to a realtime request.</param>
        /// <returns>A List(Dictionary(string, string)) describing the data at a particular time relative to the time of the request.</returns>
        public static List<Dictionary<string, string>> GetData(Google.Apis.Analytics.v3.Data.RealtimeData rt)
        {
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();

            if (!(rt.Rows == null)) {

                foreach (List<string> r in rt.Rows)
                {
                    data.Add(new Dictionary<string, string>());
                    for (int i = 0; i < r.Count; i++)
                    {
                        data[data.Count - 1].Add(rt.ColumnHeaders[i].Name, r[i]);
                    }
                }
            }

            return data;
        }
    }
}
