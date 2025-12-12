using Microsoft.AspNetCore.Mvc;
using APIrpoyecto.Services;

namespace APIrpoyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class McpController : ControllerBase
    {
        private readonly McpClientService _mcpClient;

        public McpController(McpClientService mcpClient)
        {
            _mcpClient = mcpClient;
        }

        /// <summary>
        /// Endpoint que procesa consultas en lenguaje natural usando MCP.
        /// Router: manual → LMStudio → MCP Server.
        /// </summary>
        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] McpQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Query is required.");

            try
            {
                var result = await _mcpClient.RouteQueryAsync(request.Query);
                return Ok(new { query = request.Query, result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error processing MCP query.",
                    detail = ex.Message
                });
            }
        }
    }

    public class McpQueryRequest
    {
        public string Query { get; set; }
    }
}
