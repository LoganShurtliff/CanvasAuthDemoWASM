using System.Net;

namespace TokenTestingBlazor.Client
{
    public class DatabaseAPIService
    {
        /// <summary>
        /// URI of the CosmosDB instance being accessed
        /// </summary>
        static readonly string endpoint = "YourCosmosDBURI";
        static readonly Uri baseUri = new Uri(endpoint);


        //These are currently temporary (Not really used)
        static readonly string databaseId = "test_db";
        static readonly string collectionId = "messages";
        static readonly string documentId = "0";

        static readonly string utc_date = DateTime.UtcNow.ToString("r");

        private HttpClient client;
        
        public DatabaseAPIService()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Gets the documents from a specific collection in CosmosDB
        /// </summary>
        /// <param name="token">Access Token from <see cref="AzureOAuth"/></param>
        /// <returns></returns>
        public async Task<string> ListDocumentsAsync(string token)
        {
            //Microsoft Docs here are wrong, use the location of the resource being accessed, not the location of the endpoint
            string resourceId = string.Format("dbs/{0}/colls/{1}", databaseId, collectionId);
            string resourceLink = resourceId + "/docs";

            string authHeader = GenerateAuthorizationSignature(token);

            //Add Request Headers
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", utc_date);
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);
            
            var uri = new Uri(baseUri, resourceLink);
            Console.WriteLine(uri);
            Console.WriteLine(client.DefaultRequestHeaders.ToString());
            
            try 
            { 
                var response = await client.GetStringAsync(uri);
                return response;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return ex.Message;
            }
        }

        /// <summary>
        /// Generates the Authorization Header for CosmosDB REST API calls
        /// </summary>
        /// <param name="token">Access Token from <see cref="AzureOAuth"/></param>
        /// <returns></returns>
        private static string GenerateAuthorizationSignature(string token)
        {
            var keyType = "aad";
            var tokenVersion = "1.0";

            var authSet = WebUtility.UrlEncode($"type={keyType}&ver={tokenVersion}&sig={token}");

            return authSet;
        }

    }
}
