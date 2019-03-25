using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    /// <summary>
    /// A class providing methods used to collect general data from Google Analytics. This class is abstract and must be inherited.
    /// </summary>
    abstract class GoogleDataCollector : DataCollector
    {
        /// <summary>
        /// The location of the JSON file obtained from the Google Developer Console. It contains the data in order to authenticate against the Google OAuth 2.0 API.
        /// </summary>
        public string OAuthJSONPath { get; set; } = "";
        /// <summary>
        /// The user name of the Google account that is supposed to run the app.
        /// </summary>
        public string UserName { get; set; } = "";
        /// <summary>
        /// The scopes that need to be authorized for. They are defined by Google and can be obtained using the Google API Explorer. Analytics for example require "https://www.googleapis.com/auth/analytics.readonly"
        /// </summary>
        public List<string> Scopes { get; set; } = new List<string>();

        /// <summary>
        /// Creates a new GoogleDataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="oAuthJSONPath">The location of the JSON file obtained from the Google Developer Console. It contains the data in order to authenticate against the Google OAuth 2.0 API.</param>
        /// <param name="userName">The user name of the Google account that is supposed to run the app.</param>
        public GoogleDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName)
        {
            UserName = userName;
            OAuthJSONPath = oAuthJSONPath;
        }
    }

    /// <summary>
    /// A class providing methods used to collect typed data from Google Analytics. This class is abstract and must be inherited.
    /// </summary>
    /// <typeparam name="T">The type of data (usually a DTO) that shall be collected.</typeparam>
    abstract class GoogleDataCollector<T> : GoogleDataCollector where T : class, new()
    {
        /// <summary>
        /// Creates a new DataCollector object using the Influx Database with the given name at the given endpoint to store the data using the given measurement name.
        /// </summary>
        /// <param name="databaseEndpoint">The address of an Influx Database endpoint. Usually defaults to http://localhost:8086.</param>
        /// <param name="databaseName">The name of the database the collector will save the data in.</param>
        /// <param name="measurementName">The measurement name that will be used when saving data in the database.</param>
        /// <param name="oAuthJSONPath">The location of the JSON file obtained from the Google Developer Console. It contains the data in order to authenticate against the Google OAuth 2.0 API.</param>
        /// <param name="userName">The user name of the Google account that is supposed to run the app.</param>
        public GoogleDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName, oAuthJSONPath, userName)
        {
        }
    }
}
