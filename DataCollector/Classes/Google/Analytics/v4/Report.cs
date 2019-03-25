using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
    public class Report
    {
        /// <summary>
        /// Get a string list containing the names of the requested columns.
        /// </summary>
        /// <returns>A List(string) containing the column names of the report.</returns>
        public static List<string> GetColumns(Google.Apis.AnalyticsReporting.v4.Data.Report rep)
        {
            List<string> columns = new List<string>();

            foreach (string s in rep.ColumnHeader.Dimensions)
            {
                columns.Add(s);
            }
            foreach (MetricHeaderEntry mhe in rep.ColumnHeader.MetricHeader.MetricHeaderEntries)
            {
                columns.Add(mhe.Name);
            }

            return columns;
        }

        /// <summary>
        /// Gets a list containing dictionaries each of which represents the data of one row allowing to access data by the column name.
        /// </summary>
        /// <returns>A List(Dictionary(string, string)) containing the values for each columns for each row of the report.</returns>
        public static List<Dictionary<string, string>> GetValues(Google.Apis.AnalyticsReporting.v4.Data.Report rep)
        {
            List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
            List<string> columns = GetColumns(rep);

            foreach (ReportRow row in rep.Data.Rows)
            {
                values.Add(new Dictionary<string, string>());
                for (int i = 0; i < row.Dimensions.Count; i++)
                {
                    values[values.Count - 1].Add(columns[i], row.Dimensions[i]);
                }
                for (int i = 0; i < row.Metrics[0].Values.Count; i++)
                {
                    values[values.Count - 1].Add(columns[i + row.Dimensions.Count], row.Metrics[0].Values[i]);
                }
            }

            return values;           
        }
    }
}
