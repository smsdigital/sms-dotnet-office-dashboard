using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;

namespace DataCollector
{
    class mySMS_Realtime
    {
        public static InfluxDatapoint<InfluxValueField> CreateInfluxDatapoint(string measurementName, Dictionary<string, string> fields)
        {
            InfluxDatapoint<InfluxValueField> dp = new InfluxDatapoint<InfluxValueField>();

            dp.UtcTimestamp = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                                           DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, 0).Subtract(new TimeSpan(0, Convert.ToInt32(fields["rt:minutesAgo"]), 0));
            dp.MeasurementName = measurementName;

            dp.Fields.Add("rt:pageviews", new InfluxValueField(Convert.ToInt32(fields["rt:pageviews"])));

            return dp;
        }
    }
}
