using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    class SmartAlarm_Overview
    {
        public static InfluxDatapoint<InfluxValueField> CreateInfluxDatapoint(string measurementName, DateTime timestamp, int visits, int actions, int users)
        {
            InfluxDatapoint<InfluxValueField> dp = new InfluxDatapoint<InfluxValueField>();

            dp.UtcTimestamp = timestamp;
            dp.MeasurementName = measurementName;

            dp.Fields.Add("Visits", new InfluxValueField(visits));
            dp.Fields.Add("Actions", new InfluxValueField(actions));
            dp.Fields.Add("Users", new InfluxValueField(users));

            return dp;
        }
    }
}
