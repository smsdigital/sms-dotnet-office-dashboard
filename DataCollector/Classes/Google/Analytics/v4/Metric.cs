using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
     class Metric : Google.Apis.AnalyticsReporting.v4.Data.Metric
    {
        /// <summary>
        /// Creates a new Metric with the given expression.
        /// </summary>
        /// <param name="expression">The expression of the Metric, e. g. "ga:pageviews".</param>
        public Metric(string expression)
        {
            Expression = expression;
        }
    }
}
