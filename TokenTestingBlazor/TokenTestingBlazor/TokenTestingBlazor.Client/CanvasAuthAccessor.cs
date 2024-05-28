using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Text.Json;
using TokenTestingBlazor.Client.Models;

namespace TokenTestingBlazor.Client
{
    public class CanvasAuthAccessor
    {
        private HttpClient _client;
        private readonly string authEndpoint;
        private readonly string domain = "http://localhost:3000";
        public CanvasAuthAccessor(IConfiguration Config) 
        {
            _client = new HttpClient();
            authEndpoint = Config["Canvas:auth_uri"] ?? throw new ArgumentNullException(nameof(authEndpoint));
        }

        public async Task<CanvasTokenDTO> GetAccessTokenAsync(string AuthCode)
        {
            var apiEndpoint = domain + "/api/auth/getToken?code=" + AuthCode;

            _client.DefaultRequestHeaders.Clear();

            var response = await _client.GetAsync(apiEndpoint);

            return JsonSerializer.Deserialize<CanvasTokenDTO>(response.Content.ReadAsStream());
        }

        public async Task<CanvasTokenDTO> RefreshAccessTokenAsync(string RefreshToken)
        {
            var apiEndpoint = domain + "/api/auth/refreshToken";

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("refresh_token", RefreshToken);

            var response = await _client.GetAsync(apiEndpoint);

            return JsonSerializer.Deserialize<CanvasTokenDTO>(response.Content.ReadAsStream());
        }

        public async Task CanvasLogout(string AccessToken)
        {
            string apiEndpoint = domain + "/api/auth/canvasLogout";

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("access_token", AccessToken);

            var response = await _client.DeleteAsync(apiEndpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}
