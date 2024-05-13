namespace TokenTestingBlazor.Models
{
    /// <summary>
    /// Data Transfer Object for a Canvas Token
    /// </summary>
    public class ServerCanvasTokenDTO
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string canvas_region { get; set; }
        public ServerCanvasUserDTO user { get; set; }
    }
}
