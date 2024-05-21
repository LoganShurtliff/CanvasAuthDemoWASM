using System.Text.Json;
using TokenTestingBlazor.Client.Models;

namespace TokenTestingBlazor.Client
{
    public class CanvasAuthAccessor
    {
        private HttpClient _client;
        public CanvasAuthAccessor() 
        {
            _client = new HttpClient();
        }

        public async Task<CanvasTokenDTO> GetAccessTokenAsync(string AuthCode, string AzureToken)
        {
            var apiEndpoint = "http://localhost:3000/api/auth/getToken?code=" + AuthCode;

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("AzureToken", AzureToken);

            var response = await _client.GetAsync(apiEndpoint);

            return JsonSerializer.Deserialize<CanvasTokenDTO>(response.Content.ReadAsStream());
        }

        public async Task<CanvasTokenDTO> RefreshAccessTokenAsync(string RefreshToken, string AzureToken)
        {
            var apiEndpoint = "http://localhost:3000/api/auth/refreshToken";

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("refresh_token", RefreshToken);
            _client.DefaultRequestHeaders.Add("AzureToken", AzureToken);

            var response = await _client.GetAsync(apiEndpoint);

            return JsonSerializer.Deserialize<CanvasTokenDTO>(response.Content.ReadAsStream());
        }
    }
}
