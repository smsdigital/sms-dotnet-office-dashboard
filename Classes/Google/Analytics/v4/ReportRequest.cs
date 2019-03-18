using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
    class ReportRequest : Google.Apis.AnalyticsReporting.v4.Data.ReportRequest
    {
        /// <summary>
        /// Creates a new Report request for the given view ID.
        /// </summary>
        /// <param name="viewID">The ID of the view whose data shall be requested.</param>
        public ReportRequest(string viewID) : base()
        {
            ViewId = viewID;
            Metrics = new List<Google.Apis.AnalyticsReporting.v4.Data.Metric>();
            Dimensions = new List<Google.Apis.AnalyticsReporting.v4.Data.Dimension>();
            DateRanges = new List<DateRange>();
        }

        /// <summary>
        /// Adds the metrics to the report request.
        /// </summary>
        /// <param name="metrics">A string containing all metric names that shall be requested - separated by comma.</param>
        public void AddMetrics(string metrics)
        {
            foreach(string s in metrics.Split(','))       
            {
                Metrics.Add(new Metric(s.Trim(' ')));
            }
        }

        /// <summary>
        /// Adds the metrics to the report request.
        /// </summary>
        /// <param name="metrics">An IList object containing the metric names that shall be requested.</param>
        public void AddMetrics(IList<string> metrics)
        {
            foreach(string s in metrics)
            {
                Metrics.Add(new Metric(s.Trim(' ')));
            }
        }

        /// <summary>
        /// Adds the dimensions to the report request.
        /// </summary>
        /// <param name="dimensions">A string containing all dimension names that shall be requested - separated by comma.</param>
        public void AddDimensions(string dimensions)
        {
            foreach (string s in dimensions.Split(','))
            {
                Dimensions.Add(new Dimension(s.Trim(' ')));
            }
        }

        /// <summary>
        /// Adds the metrics to the report request.
        /// </summary>
        /// <param name="dimensions">An IList object containing the dimension names that shall be requested.</param>
        public void AddDimensions(IList<string> dimensions)
        {
            foreach (string s in dimensions)
            {
                Dimensions.Add(new Dimension(s.Trim(' ')));
            }
        }

    }
}
