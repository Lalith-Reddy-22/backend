using Medtrix_PmTool_V2_Api.Interfaces.Client;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers
{
    [ApiController]
    [Route("api/v1/client")]
    public class ClientController : ControllerBase
    {
        private readonly IClient _client;

        public ClientController(IClient client)
        {
            _client = client;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _client.GetAllClientsAsync();

                return Ok(new
                {
                    clients
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}