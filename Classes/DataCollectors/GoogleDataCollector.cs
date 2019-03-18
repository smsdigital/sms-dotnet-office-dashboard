using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    abstract class GoogleDataCollector : DataCollector
    {
        public string OAuthJSONPath { get; set; } = "";
        public string UserName { get; set; } = "";
        public List<string> Scopes { get; set; } = new List<string>();

        public GoogleDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName)
        {
            UserName = userName;
            OAuthJSONPath = oAuthJSONPath;
        }
    }

    abstract class GoogleDataCollector<T> : DataCollector<T> where T : class, new()
    {
        public string OAuthJSONPath { get; set; } = "";
        public string UserName { get; set; } = "";
        public List<string> Scopes { get; set; } = new List<string>();

        public GoogleDataCollector(string databaseEndpoint, string databaseName, string measurementName, string oAuthJSONPath, string userName) : base(databaseEndpoint, databaseName, measurementName)
        {
            UserName = userName;
            OAuthJSONPath = oAuthJSONPath;
        }
    }
}
