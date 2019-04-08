using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using AdysTech.InfluxDB.Client.Net;

namespace DataCollector
{
    /// <summary>
    /// A class providing methods used collecting general data and saving it in an Influx Database. This class is abstract and must be inherited.
    /// </summary>
    abstract class DataCollector
    {
        /// <summary>
        /// The Influx client used by the collector to query the database.
        /// </summary>
        protected InfluxDBClient InfluxClient { get; set; }
        /// <summary>
        /// A timer that is used to request, proceed and save data in regular intervals.
        /// </summary>
        protected Timer Timer { get; } = new Timer();
        /// <summary>
        /// The interval in milliseconds that, when elapsed, triggers the timer to request data. Defaults to 10000.
        /// </summary>
        /// <value>10000</value>
        public int Interval { get; set; } = 10000;
        /// <summary>
        /// A value indicating whether the collector is currently running or not.
        /// </summary>
        public bool IsRunning { get; protected set; } = false;
        /// <summary>
        /// The name of the database the collector will save the data in.
        /// </summary>
        public string DatabaseName { get; set; } = "";
        /// <summary>
        /// The measurement name that will be used when saving data in the database.
        /// </summary>
        public string MeasurementName { get; set; } = "";

        /// <summary>
        /// Creates a new DataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        public DataCollector(string databaseEndpoint, string databaseName, string measurementName)
        {
            InfluxClient = new InfluxDBClient(databaseEndpoint);
            DatabaseName = databaseName;
            MeasurementName = measurementName;
            Timer.AutoReset = true;
            Timer.Elapsed += RequestData;
        }
        
        /// <summary>
        /// Requests data each time the period specified by Interval has elapsed.
        /// </summary>
        public abstract Task RequestData();

        /// <summary>
        /// Used to store data in the database by receiving a list of objects.await Those are usually DTO objects specifying the type (tag or field), the name and the data type of the parameters.
        /// </summary>
        /// <param name="data">A list of objects to be stored in the database.</param>
        protected async Task WriteToDatabase(IList<InfluxDatapoint<InfluxValueField>> data)
        {
            await InfluxClient.PostPointsAsync(DatabaseName, data);
        }

        /// <summary>
        /// A delegate matching the signature needed for the Timer.Elapsed event handler. This method is not supposed to be invoked directly.
        /// </summary>
        protected async void RequestData(object sender, ElapsedEventArgs e)
        {
            await RequestData();
        }

        /// <summary>
        /// Starts the collector
        /// </summary>
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

        /// <summary>
        /// Stops the collector.
        /// </summary>
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

    /// <summary>
    /// A  class providing methods used collecting typed data and saving it in an Influx Database. This class is abstract and must be inherited.
    /// </summary>
    /// <typeparam name="T">The type of data (usually a DTO) that shall be collected.</typeparam>
    abstract class DataCollector<T> : DataCollector where T : class, new()
    {
        /// <summary>
        /// Creates a new DataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        public DataCollector(string databaseEndpoint, string databaseName, string measurementName) : base(databaseEndpoint, databaseName, measurementName)
        {
        }
    }
}
