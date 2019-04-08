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
        /// <summary>
        /// Creates a DTO that contains SmartAlarm Analytics data to be stored in the database.
        /// </summary>
        /// <param name="measurementName">The name of the measurement that the data is stored in.</param>
        /// <param name="timestamp">The timestamp describing the aggregation of the data.</param>
        /// <param name="visits">The amount of visits for the given timestamp.</param>
        /// <param name="actions">The amount of actions for the given timestamp.</param>
        /// <param name="users">The amount of users for the given timestamp.</param>
        /// <returns>An InfluxDatapoint object containing the SmartAlarm Analytics data.</returns>
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
