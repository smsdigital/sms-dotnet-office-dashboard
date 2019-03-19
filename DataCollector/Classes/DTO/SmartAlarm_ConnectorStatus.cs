using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    class SmartAlarm_ConnectorStatus
    {
        public static InfluxDatapoint<InfluxValueField> CreateInfluxDatapoint(string measurementName, string name, bool hasHeartBeat, bool hasEvents)
        {
            InfluxDatapoint<InfluxValueField> dp = new InfluxDatapoint<InfluxValueField>();

            dp.UtcTimestamp = DateTime.UtcNow;
            dp.MeasurementName = measurementName;

            dp.Tags.Add("Name", name);
            dp.Fields.Add("HeartBeat", new InfluxValueField(Convert.ToInt32(hasHeartBeat)));
            dp.Fields.Add("Events", new InfluxValueField(Convert.ToInt32(hasEvents)));

            return dp;
        }
    }
}
