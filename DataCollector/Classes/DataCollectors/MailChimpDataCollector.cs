using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;

namespace DataCollector
{
    class MailChimpDataCollector : DataCollector
    { 
        public string APIKey { get; set; }
        public string CampaignID { get; set; }

        public MailChimpDataCollector(string databaseEndpoint, string databaseName, string measurementName,string apiKey, string campaignID) : base(databaseEndpoint, databaseName, measurementName)
        {
            APIKey = apiKey;
            CampaignID = campaignID;
        }

        public async override Task RequestData()
        {
            try {
            MailChimp.Net.MailChimpManager mailChimp = new MailChimp.Net.MailChimpManager(APIKey);
            MailChimp.Net.Models.Report report = await mailChimp.Reports.GetReportAsync(CampaignID);

            await WriteToDatabase(new List<InfluxDatapoint<InfluxValueField>>() { MailChimp_CampaignReport.CreateInfluxDatapoint(MeasurementName, report)});
            }
            catch (Exception ex) {
                Console.WriteLine("Error trying to request MailChimp data for campaign ID '" + CampaignID + "' using API key '" + APIKey + "': " + ex.Message);
            }
        }
    }
}
