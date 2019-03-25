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

        /// <summary>
        /// Creates a new GoogleRealtimeDataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="oAuthJSONPath">The location of the JSON file obtained from the Google Developer Console. It contains the data in order to authenticate against the Google OAuth 2.0 API.</param>
        /// <param name="userName">The user name of the Google account that is supposed to run the app.</param>
        public GoogleRealtimeDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName, oAuthJSONPath, userName)
        {
            Scopes.Add("https://www.googleapis.com/auth/analytics.readonly");
            AnalyticsService = Auth.GoogleAuthenticator_v3.AuthenticateByOAuth2(OAuthJSONPath, UserName, Scopes.ToArray());
            GetRequest = AnalyticsService.Data.Realtime.Get("ga:148601695", "rt:pageviews");
            GetRequest.Dimensions = "rt:minutesAgo";
        }

        /// <summary>
        /// Requests data each time the period specified by Interval has elapsed.
        /// </summary>
        public async override Task RequestData()
        {
            try {
                Google.Apis.Analytics.v3.Data.RealtimeData rt = GetRequest.Execute();
                List<Dictionary<string, string>> data = RealtimeData.GetData(rt);

                // Gets the date of the latest dataset in order to only add newer data.
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
