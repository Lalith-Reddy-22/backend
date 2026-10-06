using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Finance
{
    [Authorize(Policy = "FinanceAccess")]
    [Route("api/v1/finance-client")]
    [ApiController]
    public class FinanceClientController : ControllerBase
    {
        private readonly IFinance _financeRepository;
        private readonly IExportService _exportService;

        public FinanceClientController(IFinance financeRepository, IExportService exportService)
        {
            _financeRepository = financeRepository;
            _exportService = exportService;
        }

        // GET /api/v1/finance-client/client-gp
        [HttpGet("client-gp")]
        public async Task<IActionResult> GetClientGp(
            [FromQuery] int? finYrId = null,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _financeRepository.GetClientGpAsync(
                finYrId, searchText, pageNumber, pageSize);

            return Ok(result);
        }

        // GET /api/v1/finance-client/projects
        [HttpGet("projects")]
        public async Task<IActionResult> GetProjectsByClient(
            [FromQuery] int clientId,
            [FromQuery] int? finYrId = null,
            [FromQuery] int? projectStatus = null,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (clientId <= 0)
                return BadRequest("clientId is required.");

            var result = await _financeRepository.GetProjectsByClientAsync(
                clientId,
                finYrId,
                projectStatus,
                searchText,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        // GET: api/v1/finance-client/export/csv
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportClientCsv(
            [FromQuery] int? finYrId = null,
            [FromQuery] string? searchText = null)
        {
            var result = await _financeRepository.GetClientGpAsync(
                finYrId,
                searchText,
                1,
                int.MaxValue);

            return _exportService.ExportCsv(
                result.Clients,
                "Client_List");
        }

        // GET /api/v1/finance-client/project-task-report
        [HttpGet("project-task-report")]
        public async Task<IActionResult> GetProjectTaskReport(
            [FromQuery] int projId,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (projId <= 0)
                return BadRequest("projId is required.");

            var result = await _financeRepository.GetProjectTaskReportAsync(
                projId,
                searchText,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        // POST /api/v1/finance-client
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientRequestDto request)
        {
            request.Action = "Insert";
            request.ClientId = 0;

            var result = await _financeRepository.SaveClientAsync(request);

            if (result == "Inserted")
            {
                return Ok(new
                {
                    success = true,
                    message = "Client created successfully."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Client already exists."
            });
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetClientById(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Invalid Client Id");

            var result = await _financeRepository.GetClientByIdAsync(clientId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportClientPdf(
    [FromQuery] int? finYrId = null,
    [FromQuery] string? searchText = null)
        {
            var result = await _financeRepository.GetClientGpAsync(
                finYrId,
                searchText,
                1,
                int.MaxValue);

            return _exportService.ExportPdf(
                result.Clients,
                "Client_List");
        }

        // PUT /api/v1/finance-client/{clientId}
        [HttpPut("{clientId}")]
        public async Task<IActionResult> UpdateClient(
            int clientId,
            [FromBody] CreateClientRequestDto request)
        {
            if (clientId <= 0)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Client Id."
                });

            request.ClientId = clientId;
            request.Action = "Update";

            var result = await _financeRepository.SaveClientAsync(request);

            if (result == "Updated")
            {
                return Ok(new
                {
                    success = true,
                    message = "Client updated successfully."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result
            });
        }
    }
}