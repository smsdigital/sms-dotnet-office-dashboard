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
    class SmartAlarmDataCollector : DataCollector
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public SmartAlarmDataCollector(string databaseEndpoint, string databaseName, string measurementName, string userName, string password) : base(databaseEndpoint, databaseName, measurementName)
        {
            UserName = userName;
            Password = password;
        }

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
