using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
     class Metric : Google.Apis.AnalyticsReporting.v4.Data.Metric
    {
        public Metric(string expression)
        {
            Expression = expression;
        }
    }
}
