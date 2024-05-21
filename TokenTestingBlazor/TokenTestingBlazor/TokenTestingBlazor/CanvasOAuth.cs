using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json;
using TokenTestingBlazor.Client.Models;
using TokenTestingBlazor.Models;

namespace TokenTestingBlazor
{
    /// <summary>
    /// Utility Class for getting canvas OAuth tokens
    /// <see href="https://canvas.instructure.com/doc/api/file.oauth.html"/>
    /// </summary>
    public class CanvasOAuth
    {
        private readonly string oAuthClientID;
        private readonly string redirectURI;
        private readonly string cosmosURI;
        private readonly string tokenURI;

        private readonly HttpClient client;
        public CanvasOAuth(IConfiguration Config) 
        {
            client = new HttpClient();
            oAuthClientID = Config["Canvas:client_id"] ?? throw new ArgumentNullException(nameof(oAuthClientID));
            redirectURI = Config["Canvas:redirect_uri"] ?? throw new ArgumentNullException(nameof(redirectURI));
            cosmosURI = Config["Azure:cosmos_uri"] ?? throw new ArgumentNullException(nameof(redirectURI));
            tokenURI = Config["Canvas:token_uri"] ?? throw new ArgumentNullException(nameof(tokenURI));
        }

        /// <summary>
        /// Exchanges the Auth code for the Canvas access token, getting the client secret from the CosmosDB instance
        /// </summary>
        /// <param name="authCode">Authorization Code from canvas</param>
        /// <param name="dbToken">CosmosDB Access Token</param>
        /// <returns>A DTO containing the Canvas Access Token</returns>
        public async Task<ServerCanvasTokenDTO> GetCanvasTokenAsync(string authCode, string dbToken)
        {
            var endpoint = new Uri(tokenURI);

            string client_secret = await GetClientSecretAsync(dbToken);

            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", oAuthClientID },
                { "client_secret", client_secret },
                { "redirect_uri", redirectURI },
                {"code", authCode },
            };

            var requestContent = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(endpoint.ToString(), requestContent);
            response.EnsureSuccessStatusCode();


            
            return JsonSerializer.Deserialize<ServerCanvasTokenDTO>(response.Content.ReadAsStream());
        }

        /// <summary>
        /// Refreshes the Canvas Access Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="dbToken"></param>
        /// <returns></returns>
        public async Task<ServerCanvasRefreshDTO> RefreshCanvasTokenAsync(string refreshToken, string dbToken)
        {
            var endpoint = new Uri(tokenURI);
            string client_secret = await GetClientSecretAsync(dbToken);

            var values = new Dictionary<string, string>()
            {
                {"grant_type", "refresh_token" },
                {"client_id", oAuthClientID },
                { "client_secret", client_secret },
                { "redirect_uri", redirectURI },
                { "refresh_token", refreshToken },
            };

            var requestContent = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(endpoint.ToString(), requestContent);
            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<ServerCanvasRefreshDTO>(response.Content.ReadAsStream());
        }

        /// <summary>
        /// Gets the Canvas OAuth Client Secret from the CosmosDB instance
        /// </summary>
        /// <param name="token">CosmosDB Access Token</param>
        /// <returns>OAuth Client Secret</returns>
        public async Task<string> GetClientSecretAsync(string token)
        {
            var baseUri = new Uri(cosmosURI);

            //Microsoft Docs here are wrong, use the location of the resource being accessed, not the location of the endpoint
            
            string resourceId = string.Format("dbs/{0}/colls/{1}", "test_db", "oauth"); //Resource URI (test_db is the database, oauth is the collection)
            string resourceLink = resourceId + "/docs";

            string authHeader = WebUtility.UrlEncode($"type=aad&ver=1.0&sig={token}");

            //Add Request Headers
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", DateTime.UtcNow.ToString("r"));
            client.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            client.DefaultRequestHeaders.Add("Authorization", authHeader);

            var uri = new Uri(baseUri, resourceLink);
            Console.WriteLine(uri);
            Console.WriteLine(client.DefaultRequestHeaders.ToString());

            var response = await client.GetAsync(uri);
            var docList = JsonSerializer.Deserialize<CosmosDBDocumentList>(response.Content.ReadAsStream());
            var credentials = docList.Documents[0]; //Access the first document in the list, assumes the only document in the collection is what contains the client id and secret
            return credentials.client_secret;
        }

        public record CosmosDBDocumentList
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string _rid { get; set; }
            public int _count { get; set; }
            public CosmosDBDocument[] Documents { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. 
        }

        //JSON should be
        /*
        {
            "id": "0",
            "client_id": "yourclientid",
            "client_secret": "yourclientsecret"
        }
         */
        public record CosmosDBDocument
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string id { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string _rid { get; set; }
            public string _self { get; set; }
            public string _etag { get; set; }
            public int _ts { get; set; }
            public string _attachments { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.        
        }

    }
}
