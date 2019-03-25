using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
    class GetReportsResponse
    {
        /// <summary>
        /// Gets the columns of a Google Analytics response.
        /// </summary>
        /// <param name="resp">The Google Analytics response whose columns shall be returned.</param>
        /// <returns>A List(List(string)) containing the columns for each Report.</returns>
        public static List<List<string>> GetColumns(Google.Apis.AnalyticsReporting.v4.Data.GetReportsResponse resp)
        {
            List<List<string>> columns = new List<List<string>>();

            foreach (Google.Apis.AnalyticsReporting.v4.Data.Report rep in resp.Reports)
            {
                columns.Add(Report.GetColumns(rep));
            }

            return columns;
        }

        /// <summary>
        /// Gets the values of a Google Analytics response.
        /// </summary>
        /// <param name="resp">The Google Analytics response whose values shall be returned.</param>
        /// <returns>A List(List(Dictionary(string, string)) containing the values for each column for each row for each Report.</returns>
        public static List<List<Dictionary<string, string>>> GetValues(Google.Apis.AnalyticsReporting.v4.Data.GetReportsResponse resp)
        {
            List<List<Dictionary<string, string>>> values = new List<List<Dictionary<string, string>>>();

            foreach (Google.Apis.AnalyticsReporting.v4.Data.Report rep in resp.Reports)
            {
                values.Add(Report.GetValues(rep));
            }

            return values;
        }
    }
}
