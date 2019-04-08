using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;

namespace DataCollector
{
    class mySMS_Overview
    {
        /// <summary>
        /// Creates a DTO that contains Google Analytics data to be stored in the database.
        /// </summary>
        /// <param name="measurementName">The name of the measurement that the data is stored in.</param>
        /// <param name="fields">A dictionary containing the requested Google Analytics data.</param>
        /// <returns>An InfluxDatapoint object containing the Google Analytics data.</returns>
        public static InfluxDatapoint<InfluxValueField> CreateInfluxDatapoint(string measurementName, Dictionary<string, string> fields)
        {
            InfluxDatapoint<InfluxValueField> dp = new InfluxDatapoint<InfluxValueField>();
            
            dp.UtcTimestamp = new DateTime(Convert.ToInt32(fields["ga:year"]), Convert.ToInt32(fields["ga:month"]), Convert.ToInt32(fields["ga:day"]), 0, 0, 0);
            dp.MeasurementName = measurementName;

            dp.Fields.Add("ga:pageviews", new InfluxValueField(Convert.ToInt32(fields["ga:pageviews"])));
            dp.Fields.Add("ga:sessions", new InfluxValueField(Convert.ToInt32(fields["ga:sessions"])));
            dp.Fields.Add("ga:users", new InfluxValueField(Convert.ToInt32(fields["ga:users"])));

            return dp;
        }
    }
}
