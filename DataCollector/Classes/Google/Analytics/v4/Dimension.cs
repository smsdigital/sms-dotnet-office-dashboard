using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
    class Dimension : Google.Apis.AnalyticsReporting.v4.Data.Dimension
    {
        /// <summary>
        /// Creates a new Dimension with the given expression.
        /// </summary>
        /// <param name="expression">The expression of the Dimension, e. g. "ga:day".</param>
        public Dimension(string expression)
        {
            Name = expression;
        }
    }
}
