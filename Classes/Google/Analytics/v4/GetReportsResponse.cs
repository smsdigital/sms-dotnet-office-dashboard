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
        public static List<List<string>> GetColumns(Google.Apis.AnalyticsReporting.v4.Data.GetReportsResponse resp)
        {
            List<List<string>> columns = new List<List<string>>();

            foreach (Google.Apis.AnalyticsReporting.v4.Data.Report rep in resp.Reports)
            {
                columns.Add(Report.GetColumns(rep));
            }

            return columns;
        }

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
