using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using AdysTech.InfluxDB.Client.Net;

namespace DataCollector
{
    /// <summary>
    /// A collector for Matomo data.
    /// </summary>
    class MatomoDataCollector : DataCollector {
        /// <summary>
        /// The token to authenticate against the Matomo API.await The token is related to a user account and can access data according to the privileges of the user account respectively.
        /// </summary>
        private string Token { get; set; }

        private const string matomoURL = "http://matomo.sms-digital-labs.com/?module=API";

        /// <summary>
        /// Creates a new SmartAlarmDataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name. it requests data using the given token.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="token">The token to authenticate against the Matomo API.await The token is related to a user account and can access data according to the privileges of the user account respectively.</param>
        public MatomoDataCollector(string databaseEndpoint, string databaseName, string measurementName, string token) : base(databaseEndpoint, databaseName, measurementName) {
            Token = token;
        }

        /// <summary>
        /// Requests data each time the period specified by Interval has elapsed.
        /// </summary>
        public async override Task RequestData() {
            try {
                string reqURL = string.Format("{0}&method={1}&idSite={2}&date={3}&period={4}&format={5}&token_auth={6}",
                    matomoURL,
                    "VisitsSummary.get",
                    "4",
                    DateTime.Now.AddDays(-49).ToString("yyyy-MM-dd") + ",today",
                    "day",
                    "json",
                    Token
                );
                HttpWebRequest req = WebRequest.CreateHttp(reqURL);
                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();

                string json = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                resp.Close();
                await WriteToDatabase(ParseJSON(json));
            }
            catch (Exception ex) {
                Console.WriteLine("Error trying to request Matomo data: " + ex.Message);
            }
        }

        /// <summary>
        /// Parses the given JSON string in order to extract the Matomo Analytics data.
        /// </summary>
        /// <param name="json">The JSON string to be parsed.</param>
        /// <returns>A list containing the data for each day.</returns>
        private List<InfluxDatapoint<InfluxValueField>> ParseJSON(string json)
        {
            JObject o = JObject.Parse(json);

            List<InfluxDatapoint<InfluxValueField>> list = new List<InfluxDatapoint<InfluxValueField>>();
            foreach (JToken c in o.Children()) {
                if (c.First.Count() > 0) {
                    int visits = c.First.SelectToken("nb_visits").Value<int>();
                    int actions = c.First.SelectToken("nb_actions").Value<int>();
                    int users = c.First.SelectToken("nb_users").Value<int>();
                    list.Add(SmartAlarm_Overview.CreateInfluxDatapoint(MeasurementName, 
                    DateTime.Parse(c.Path).AddMinutes(TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes).ToUniversalTime(), visits, actions, users));
                }
            }

            return list;
        }
    }
}