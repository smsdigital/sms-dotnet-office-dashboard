using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;

namespace DataCollector
{
    /// <summary>
    /// A data collector for MailChimp data.
    /// </summary>
    class MailChimpDataCollector : DataCollector
    { 
        /// <summary>
        /// The APIKey used to request data from the MailChimp API. See https://mailchimp.com/help/about-api-keys/.
        /// </summary>
        public string APIKey { get; set; }
        /// <summary>
        /// The ID of the campaign whose data shall be requested.
        /// </summary>
        public string CampaignID { get; set; }

        /// <summary>
        /// Creates a new MailChimpDataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name. It collects data about the MailChimp campaign with the given ID using the given API key.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="apiKey">The API key obtained from MailChimp. See https://mailchimp.com/help/about-api-keys/.</param>
        /// <param name="campaignID">The ID of the campaign whose data shall be requested.</param>
        /// <returns></returns>
        public MailChimpDataCollector(string databaseEndpoint, string databaseName, string measurementName,string apiKey, string campaignID) : base(databaseEndpoint, databaseName, measurementName)
        {
            APIKey = apiKey;
            CampaignID = campaignID;
        }

        /// <summary>
        /// Requests data each time the period specified by Interval has elapsed.
        /// </summary>
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
