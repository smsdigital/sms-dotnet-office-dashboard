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
        /// <summary>
        /// Creates a DTO that contains SmartAlarm connector data to be stored in the database.
        /// </summary>
        /// <param name="measurementName">The name of the measurement that the data is stored in.</param>
        /// <param name="name">The name of the connector.</param>
        /// <param name="hasHeartBeat">A value indicating whether the connector has heartbeat or not.</param>
        /// <param name="events">A value indicating whether the connector is sending events or not.</param>
        /// <returns>An InfluxDatapoint object containing the SmartAlarm connector data.</returns>
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
