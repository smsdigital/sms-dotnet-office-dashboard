using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace DataCollector
{
    class Program
    {
        static void Main(string[] args)
        {          
            IConfiguration config = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("settings.json").Build();

            GoogleAnalyticsDataCollector<mySMS_Overview> coll = new GoogleAnalyticsDataCollector<mySMS_Overview>(config["DatabaseEndpoint"], config["DatabaseName"], "mySMS_Overview", config["GoogleAuthenticationJSONPath"], config["GoogleAuthenticationUserName"]);
            GoogleRealtimeDataCollector<mySMS_Realtime> coll2 = new GoogleRealtimeDataCollector<mySMS_Realtime>(config["DatabaseEndpoint"], config["DatabaseName"], "mySMS_Realtime", config["GoogleAuthenticationJSONPath"], config["GoogleAuthenticationUserName"]);
            MailChimpDataCollector coll3 = new MailChimpDataCollector(config["DatabaseEndpoint"], config["DatabaseName"], "MailChimp", config["MailChimpAPIKey"], config["MailChimpCampaignID"]);
            SmartAlarmDataCollector coll4 = new SmartAlarmDataCollector(config["DatabaseEndpoint"], config["DatabaseName"], "SmartAlarm_ConnectorStatus", config["SmartAlarmConnectorStatusUserName"], config["SmartAlarmConnectorStatusPassword"]);

            try
            {
                Analytics.v4.ReportRequest req = new Analytics.v4.ReportRequest("ga:148601695");
                req.DateRanges.Add(new DateRange()
                {
                    StartDate = DateTime.Now.AddMonths(-1).AddDays(-1).ToString("yyyy-MM-dd"),
                    EndDate = DateTime.Now.ToString("yyyy-MM-dd")
                });
                req.AddMetrics("ga:pageviews,ga:sessions,ga:users");
                req.AddDimensions("ga:year,ga:month,ga:day");

                coll.GetReportsRequest.ReportRequests.Add(req);
                coll.Interval = 3600 * 1000;
                coll.Start();

                coll2.Interval = 60 * 1000;
                coll2.Start();

                coll3.Interval = 900 * 1000;
                coll3.Start();

                coll4.Interval = 900 * 1000;
                coll4.Start();

                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
