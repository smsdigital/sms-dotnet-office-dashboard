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
    class MatomoDataCollector : DataCollector {
        private string Token { get; set; }
        private const string matomoURL = "http://matomo.sms-digital-labs.com?module=API";

        public MatomoDataCollector(string databaseEndpoint, string databaseName, string measurementName, string token) : base(databaseEndpoint, databaseName, measurementName) {
            Token = token;
        }

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

        private List<InfluxDatapoint<InfluxValueField>> ParseJSON(string json)
        {
            JObject o = JObject.Parse(json);

            List<InfluxDatapoint<InfluxValueField>> list = new List<InfluxDatapoint<InfluxValueField>>();
            foreach (JToken c in o.Children()) {
                int visits = c.First.SelectToken("nb_visits").Value<int>();
                int actions = c.First.SelectToken("nb_actions").Value<int>();
                int users = c.First.SelectToken("nb_users").Value<int>();
                list.Add(SmartAlarm_Overview.CreateInfluxDatapoint(MeasurementName, DateTime.Parse(c.Path).ToUniversalTime(), visits, actions, users));
            }

            return list;
        }
    }
}