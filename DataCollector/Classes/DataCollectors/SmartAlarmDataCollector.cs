using AdysTech.InfluxDB.Client.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    /// <summary>
    /// A collector for data about SMS digital's SmartAlarm connectors.
    /// /// </summary>
    class SmartAlarmDataCollector : DataCollector
    {
        /// <summary>
        /// The user name to authenticate against the API.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The password to authenticate against the API.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Creates a new SmartAlarmDataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name. It authenticates using the given username and password.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="userName">The user name to authenticate against the API.</param>
        /// <param name="password">The password to authenticate against the API.</param>
        /// <returns></returns>
        public SmartAlarmDataCollector(string databaseEndpoint, string databaseName, string measurementName, string userName, string password) : base(databaseEndpoint, databaseName, measurementName)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Requests data each time the period specified by Interval has elapsed.
        /// </summary>
        public async override Task RequestData()
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpWebRequest httpReq = WebRequest.CreateHttp("https://api.smart-alarm.my.sms-group.com/status/connectors");
                httpReq.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(UserName + ":" + Password)));

                HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
                string json = new StreamReader(httpResp.GetResponseStream()).ReadToEnd();
                httpResp.Close();
                await WriteToDatabase(ParseJSON(json));
            }
            catch (WebException ex)
            {
                try
                {
                    // The response may be 500 if one of the connectors is not working properly. This is intended behavior, so even in this case data has to be proceeded.
                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.InternalServerError)
                    {
                        string json = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()).ReadToEnd();
                        ex.Response.Close();
                        if (json != "")
                        {
                            await WriteToDatabase(ParseJSON(json));
                        }
                    }
                }
                catch (Exception inEx)
                {
                    Console.WriteLine("Error trying to request SmartAlarm connector data: " + inEx.Message);
                }
            }
        }

        /// <summary>
        /// Parses the given JSON string in order to extract the connector name and its heartbeat and event data.
        /// </summary>
        /// <param name="json">The JSON string to be parsed.</param>
        /// <returns>A list containing the data for each SmartAlarm connector.</returns>
        private List<InfluxDatapoint<InfluxValueField>> ParseJSON(string json)
        {
            JObject o = JObject.Parse(json);

            if (o.SelectToken("error") != null)
                return null;

            List<InfluxDatapoint<InfluxValueField>> list = new List<InfluxDatapoint<InfluxValueField>>();

            foreach (JProperty connector in o.Properties())
            {
                string name = connector.Name;
                bool heartbeat = o.SelectToken(name).SelectToken("heartbeat").SelectToken("valid").Value<bool>();
                bool events = o.SelectToken(name).SelectToken("events").SelectToken("valid").Value<bool>();

                list.Add(SmartAlarm_ConnectorStatus.CreateInfluxDatapoint(MeasurementName, name, heartbeat, events));
            }

            return list;
        }
    }
}
