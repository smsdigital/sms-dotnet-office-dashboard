using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using AdysTech.InfluxDB.Client.Net;

namespace DataCollector
{
    abstract class DataCollector
    {
        protected InfluxDBClient InfluxClient { get; set; }
        protected Timer Timer { get; } = new Timer();
        public int Interval { get; set; } = 10000;
        public bool IsRunning { get; protected set; } = false;
        public string DatabaseName { get; set; } = "";
        public string MeasurementName { get; set; } = "";


        public DataCollector(string databaseEndpoint, string databaseName, string measurementName)
        {
            InfluxClient = new InfluxDBClient(databaseEndpoint);
            DatabaseName = databaseName;
            MeasurementName = measurementName;
            Timer.AutoReset = true;
            Timer.Elapsed += RequestData;
        }

        public abstract Task RequestData();

        protected async Task WriteToDatabase(IList<InfluxDatapoint<InfluxValueField>> data)
        {
            await InfluxClient.PostPointsAsync(DatabaseName, data);
        }

        protected async void RequestData(object sender, ElapsedEventArgs e)
        {
            await RequestData();
        }

        public void Start()
        {
            if (!IsRunning)
            {
                RequestData();
                Timer.Interval = Interval;
                Timer.Start();
                IsRunning = true;
                Console.WriteLine("Collector is starting...");
            }
            else
            {
                Console.WriteLine("Collector has already been started!");
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                Timer.Stop();
                IsRunning = false;
                Console.WriteLine("Collector is stopping...");
            }
            else
            {
                Console.WriteLine("Collector has already been stopped!");
            }
        }
    }

    abstract class DataCollector<T> : DataCollector where T : class, new()
    {
        public DataCollector(string databaseEndpoint, string databaseName, string measurementName) : base(databaseEndpoint, databaseName, measurementName)
        {
        }
    }
}
