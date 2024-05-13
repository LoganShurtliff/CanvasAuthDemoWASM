using Microsoft.AspNetCore.Mvc;
using TokenTestingBlazor.Models;

namespace TokenTestingBlazor.Controllers
{
    /// <summary>
    /// API Controller used for Canvas Authentication
    /// </summary>
    [Route("api/auth/getToken")]
    [ApiController]
    public class CanvasAuthController : ControllerBase
    {

        private readonly CanvasOAuth _canvasAuth;
        public CanvasAuthController(CanvasOAuth canvasAuth) 
        {
            _canvasAuth = canvasAuth;
        }

        //GET: api/auth/getToken
        /// <summary>
        /// API Endpoint to exchange the Canvas Auth Code for an Access Token
        /// </summary>
        /// <param name="code">Canvas Auth Code</param>
        /// <param name="AzureToken">CosmosDB Access Token</param>
        /// <returns>Canvas Access Token</returns>
        [HttpGet]
        public async Task<ActionResult<ServerCanvasTokenDTO>> GetCanvasToken(string code, [FromHeader] string AzureToken)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NotFound();
            }

            var token = await _canvasAuth.GetCanvasTokenAsync(code, AzureToken);
            return token;
        }
    }
}
