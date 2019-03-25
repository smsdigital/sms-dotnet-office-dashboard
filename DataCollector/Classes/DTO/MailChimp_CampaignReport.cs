using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    class MailChimp_CampaignReport
    {
        /// <summary>
        /// Creates a DTO that contains MailChimp campaign data to be stored in the database.
        /// </summary>
        /// <param name="measurementName">The name of the measurement that the data is stored in.</param>
        /// <param name="report">The MailChimp report whose data shall be stored.</param>
        /// <returns>An InfluxDatapoint object containing the MailChimp report data.</returns>
        public static InfluxDatapoint<InfluxValueField> CreateInfluxDatapoint(string measurementName, MailChimp.Net.Models.Report report)
        {
            InfluxDatapoint<InfluxValueField> dp = new InfluxDatapoint<InfluxValueField>();

            dp.UtcTimestamp = DateTime.UtcNow;
            dp.MeasurementName = measurementName;
            
            dp.Tags.Add("CampaignID", report.Id);
            dp.Fields.Add("Bounces", new InfluxValueField(report.Bounces.SoftBounces + report.Bounces.HardBounces));
            dp.Fields.Add("EMailsSent", new InfluxValueField(report.EmailsSent));
            dp.Fields.Add("Forwards", new InfluxValueField(report.Forwards.ForwardsCount));
            dp.Fields.Add("UniqueClicks", new InfluxValueField(report.Clicks.UniqueSubscriberClicks));
            dp.Fields.Add("UniqueOpens", new InfluxValueField(report.Opens.UniqueOpens));
            dp.Fields.Add("Unsubscribed", new InfluxValueField(report.Unsubscribed));

            return dp;
        }
    }
}
