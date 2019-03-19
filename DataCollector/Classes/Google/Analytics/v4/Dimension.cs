using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Analytics.v4
{
    class Dimension : Google.Apis.AnalyticsReporting.v4.Data.Dimension
    {
        public Dimension(string expression)
        {
            Name = expression;
        }
    }
}
