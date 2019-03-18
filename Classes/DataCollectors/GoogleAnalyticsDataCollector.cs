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
    class GoogleAnalyticsDataCollector<T> : GoogleDataCollector<T> where T : class, new()
    {
        public AnalyticsReportingService AnalyticsReportingService { get; set; }
        public GetReportsRequest GetReportsRequest { get; set; } = new GetReportsRequest();
        public GetReportsResponse GetReportsResponse { get; set; }

        public GoogleAnalyticsDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName, oAuthJSONPath, userName)
        {
            Scopes.Add("https://www.googleapis.com/auth/analytics.readonly");
            AnalyticsReportingService = Auth.GoogleAuthenticator.AuthenticateByOAuth2(OAuthJSONPath, UserName, Scopes.ToArray());
            GetReportsRequest.ReportRequests = new List<ReportRequest>();
        }

        public async override Task RequestData()
        {
            try {
                GetReportsResponse = AnalyticsReportingService.Reports.BatchGet(GetReportsRequest).Execute();

                List<string> columns = Analytics.v4.GetReportsResponse.GetColumns(GetReportsResponse)[0];
                List<List<Dictionary<string, string>>> values = Analytics.v4.GetReportsResponse.GetValues(GetReportsResponse);

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
