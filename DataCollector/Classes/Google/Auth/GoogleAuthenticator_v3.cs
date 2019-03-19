using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollector.Auth
{
    class GoogleAuthenticator_v3
    {
        /// <summary>
        /// Provides a valid AnalyticsReportingService object for a public API Key.
        /// </summary>
        /// <param name="APIKey">An API key from the Google Developer Console.</param>
        /// <returns>An AnalyticsReportingService object able to access data according to the privileges of the API key.</returns>
        public static AnalyticsService AuthenticateByAPIKey(string APIKey)
        {
            try
            {
                if (string.IsNullOrEmpty(APIKey))
                    throw new ArgumentNullException("APIKey", "The Google API key must not be null or empty.");

                return new AnalyticsService(new BaseClientService.Initializer()
                {
                    ApiKey = APIKey,
                    ApplicationName = Process.GetCurrentProcess().ProcessName,
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create new AnalyticsReportingService", ex);
            }
        }

        /// <summary>
        /// Provides a valid AnalyticsReportingService object that authenticates using OAuth 2.0 credentials.
        /// </summary>
        /// <param name="jsonPath">The path to the OAuth client ID JSON file from the Google Developer Console.</param>
        /// <param name="userName">The user name that wants to be authenticated.</param>
        /// <param name="scopes">The scopes the respective user wants to be granted for.</param>
        /// <returns></returns>
        public static AnalyticsService AuthenticateByOAuth2(string jsonPath, string userName, string[] scopes)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ArgumentNullException("userName", "The user name must not be null or empty.");
                if (string.IsNullOrEmpty(jsonPath))
                    throw new ArgumentNullException("jsonPath", "The path to the JSON file must not be null or empty.");
                if (!File.Exists(jsonPath))
                    throw new Exception("jsonPath: The value is not a path or the path is not valid: " + jsonPath);

                var cred = GetUserCredential(jsonPath, userName, scopes);
                return GetService(cred);
            }
            catch (Exception ex)
            {
                throw new Exception("Get AnalyticsReportingService failed.", ex);
            }
        }

        /// <summary>
        /// Gets the user credentials from the OAuth 2.0 API using the client data from the JSON file and and grants the given user access to the requested scopes.
        /// </summary>
        /// <param name="jsonPath">The path to the OAuth client ID JSON file from the Google Developer Console.</param>
        /// <param name="userName">The user name that wants to be authenticated.</param>
        /// <param name="scopes">The scopes the respective user wants to be granted for.</param>
        /// <returns>A UserCredential object to use for OAuth 2.0 authentication.</returns>
        private static UserCredential GetUserCredential(string jsonPath, string userName, string[] scopes)
        {
            try
            {
                // These are the scopes of permissions you need. It is best to request only what you need and not all of them               
                using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

                    // Requesting Authentication or loading previously stored authentication for userName
                    var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                                                                             scopes,
                                                                             userName,
                                                                             CancellationToken.None,
                                                                             new FileDataStore(credPath, true)).Result;

                    credential.GetAccessTokenForRequestAsync();
                    return credential;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Get user credentials failed.", ex);
            }
        }

        /// <summary>
        /// Gets an AnalyticsReportingService object that is able to request data from the Analytics Reporting API with the given user credentials.
        /// </summary>
        /// <param name="credential">An UserCredentials object that contains authenticated user credentials.</param>
        /// <returns>An AnalyticsReportingService object able to request data against the Analytics Reporting API.</returns>
        private static AnalyticsService GetService(UserCredential credential)
        {
            try
            {
                if (credential == null)
                    throw new ArgumentNullException("credential");

                // Create Analyticsreporting API service.
                return new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Process.GetCurrentProcess().ProcessName
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Get Analyticsreporting service failed.", ex);
            }
        }

        /// <summary>
        /// Provides a valid AnalyticsReportingService object that authenticates using a service account.
        /// Documentation: https://developers.google.com/accounts/docs/OAuth2#serviceaccount
        /// </summary>
        /// <param name="email">The e-mail address of the service account; can be obtained from the Google Developer Console.</param>
        /// <param name="credentialFilePath">The path of the JSON or p12 file downloaded from the Google Developer Console.</param>
        /// <returns>An AnalyticsReportingService object able to request data against the Analytics Reporting API.</returns>
        public static AnalyticsService AuthenticateByServiceAccount(string email, string credentialFilePath, string[] scopes)
        {
            try
            {
                if (string.IsNullOrEmpty(credentialFilePath))
                    throw new Exception("The path to the JSON or p12 file must not be null or empty.");
                if (!File.Exists(credentialFilePath))
                    throw new Exception("The value is not a path or the path is not valid.");
                if (string.IsNullOrEmpty(email))
                    throw new Exception("Service account email is required.");

                // For Json file
                if (Path.GetExtension(credentialFilePath).ToLower() == ".json")
                {
                    GoogleCredential credential;
                    using (var stream = new FileStream(credentialFilePath, FileMode.Open, FileAccess.Read))
                    {
                        credential = GoogleCredential.FromStream(stream)
                             .CreateScoped(scopes);
                    }

                    // Create the  Analytics service.
                    return new AnalyticsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = Process.GetCurrentProcess().ProcessName,
                    });
                }
                else if (Path.GetExtension(credentialFilePath).ToLower() == ".p12")
                {   // If its a P12 file

                    var certificate = new X509Certificate2(credentialFilePath, "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                    var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(email)
                    {
                        Scopes = scopes
                    }.FromCertificate(certificate));

                    // Create the  Analyticsreporting service.
                    return new AnalyticsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = Process.GetCurrentProcess().ProcessName,
                    });
                }
                else
                {
                    throw new Exception("Unsupported Service accounts credentials.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("CreateServiceAccountAnalyticsreportingFailed", ex);
            }
        }
    }
}
