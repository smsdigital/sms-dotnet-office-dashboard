using AdysTech.InfluxDB.Client.Net;
using DataCollector.Analytics.v3;
using Google.Apis.Analytics.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollector
{
    class GoogleRealtimeDataCollector<T> : GoogleDataCollector<T> where T : class, new()
    {
        public AnalyticsService AnalyticsService { get; set; }
        public DataResource.RealtimeResource.GetRequest GetRequest { get; set; }

        public GoogleRealtimeDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName, oAuthJSONPath, userName)
        {
            Scopes.Add("https://www.googleapis.com/auth/analytics.readonly");
            AnalyticsService = Auth.GoogleAuthenticator_v3.AuthenticateByOAuth2(OAuthJSONPath, UserName, Scopes.ToArray());
            GetRequest = AnalyticsService.Data.Realtime.Get("ga:148601695", "rt:pageviews");
            GetRequest.Dimensions = "rt:minutesAgo";
        }

        public async override Task RequestData()
        {
            try {
                Google.Apis.Analytics.v3.Data.RealtimeData rt = GetRequest.Execute();
                List<Dictionary<string, string>> data = RealtimeData.GetData(rt);

                List<IInfluxSeries> last = await InfluxClient.QueryMultiSeriesAsync(DatabaseName, "SELECT last(*) FROM " + MeasurementName);

                DateTime now = DateTime.UtcNow;
                DateTime New = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                DateTime? latest = last.FirstOrDefault()?.Entries[0].Time;

                List<InfluxDatapoint<InfluxValueField>> list = new List<InfluxDatapoint<InfluxValueField>>();
                foreach (Dictionary<string, string> dict in data)
                {
                    if (latest == null)
                    {
                        list.Add((InfluxDatapoint<InfluxValueField>)typeof(T).GetMethod("CreateInfluxDatapoint").Invoke(null, new object[] { MeasurementName, dict }));
                    }
                    else
                    {
                        // Necessary since nanoseconds cannot be set and will cause one date to be greater than the other although all (visible) parameters are equal.
                        if (New.Subtract(new TimeSpan(0, Convert.ToInt32(dict["rt:minutesAgo"]), 0)) > latest)
                        {
                            list.Add((InfluxDatapoint<InfluxValueField>)typeof(T).GetMethod("CreateInfluxDatapoint").Invoke(null, new object[] { MeasurementName, dict }));
                        }
                    }
                    await WriteToDatabase(list);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error trying to request Google Analytics data: " + ex.Message);
            }
        }
    }
}
