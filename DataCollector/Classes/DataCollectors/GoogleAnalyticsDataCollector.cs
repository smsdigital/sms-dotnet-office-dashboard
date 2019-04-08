using AdysTech.InfluxDB.Client.Net;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataCollector
{
    /// <summary>
    /// A collector for Google Analytics data.
    /// </summary>
    /// <typeparam name="T">The type of data (usually a DTO) that shall be collected.</typeparam>
    class GoogleAnalyticsDataCollector<T> : GoogleDataCollector<T> where T : class, new()
    {
        /// <summary>
        /// The Google Analytics Reporting Service that authenticates against the Google OAuth 2.0 API and can then be used to request data with the given credentials.
        /// </summary>
        public AnalyticsReportingService AnalyticsReportingService { get; set; }
        /// <summary>
        /// A GetReportsRequest object used tgo request data from Google Analytics.
        /// </summary>
        public GetReportsRequest GetReportsRequest { get; set; } = new GetReportsRequest();
        /// <summary>
        /// A GetReportsResponse object containing data requested by GetReportsRequest.
        /// </summary>
        public GetReportsResponse GetReportsResponse { get; set; }

        /// <summary>
        /// Creates a new GoogleAnalyticsDataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="oAuthJSONPath">The location of the JSON file obtained from the Google Developer Console. It contains the data in order to authenticate against the Google OAuth 2.0 API.</param>
        /// <param name="userName">The user name of the Google account that is supposed to run the app.</param>
        public GoogleAnalyticsDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName, oAuthJSONPath, userName)
        {
            Scopes.Add("https://www.googleapis.com/auth/analytics.readonly");
            AnalyticsReportingService = Auth.GoogleAuthenticator.AuthenticateByOAuth2(OAuthJSONPath, UserName, Scopes.ToArray());
            GetReportsRequest.ReportRequests = new List<ReportRequest>();
        }

        /// <summary>
        /// Requests data each time the period specified by Interval has elapsed.
        /// </summary>
        public async override Task RequestData()
        {
            try {
                GetReportsResponse = AnalyticsReportingService.Reports.BatchGet(GetReportsRequest).Execute();

                List<string> columns = Analytics.v4.GetReportsResponse.GetColumns(GetReportsResponse)[0];
                List<List<Dictionary<string, string>>> values = Analytics.v4.GetReportsResponse.GetValues(GetReportsResponse);

                // Gets the date of the latest dataset in order to only add newer data.
                List<IInfluxSeries> last = await InfluxClient.QueryMultiSeriesAsync(DatabaseName, "SELECT last(*) FROM " + MeasurementName);
                DateTime now = DateTime.UtcNow;
                DateTime New = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                DateTime? latest = last.FirstOrDefault()?.Entries[0].Time;

                foreach (List<Dictionary<string, string>> report in values)
                {
                    List<InfluxDatapoint<InfluxValueField>> list = new List<InfluxDatapoint<InfluxValueField>>();
                    foreach (Dictionary<string, string> row in report)
                    {
                        if (latest == null)
                        {
                            list.Add((InfluxDatapoint<InfluxValueField>)typeof(T).GetMethod("CreateInfluxDatapoint").Invoke(null, new object[] { MeasurementName, row }));
                        }
                        else
                        {
                            New = new DateTime(Convert.ToInt32(row["ga:year"]), Convert.ToInt32(row["ga:month"]), Convert.ToInt32(row["ga:day"]), 0, 0, 0);
                            if (New > latest)
                            {
                                list.Add((InfluxDatapoint<InfluxValueField>)typeof(T).GetMethod("CreateInfluxDatapoint").Invoke(null, new object[] { MeasurementName, row }));
                            }
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
