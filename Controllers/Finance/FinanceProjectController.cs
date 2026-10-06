using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Finance
{
    [Authorize(Policy = "FinanceAccess")]
    [Route("api/v1/finance-project")]
    [ApiController]
    public class FinanceProjectController : ControllerBase
    {
        private readonly IFinanceProject _repo;

        public FinanceProjectController(IFinanceProject repo)
        {
            _repo = repo;
        }

        /// GET /api/v1/finance-project/lookups
        /// Returns all dropdowns : clients, PMs, buying centers, financial years, project statuses
        [HttpGet("lookups")]
        public async Task<IActionResult> GetProjectLookups()
            => Ok(await _repo.GetProjectLookupsAsync());

        /// GET /api/v1/finance-project/{projectId}
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById([FromRoute] int projectId)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");

            var result = await _repo.GetProjectByIdAsync(projectId);
            if (result == null) return NotFound(new { message = "Project not found." });

            return Ok(result);
        }

        /// POST /api/v1/finance-project
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] SaveProjectRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ProjectTitle))
                return BadRequest("projectTitle is required.");
            if (request.ClientId <= 0)
                return BadRequest("clientId is required.");

            var result = await _repo.SaveProjectAsync("Insert", null, request);

            if (!result.Success)
                return Conflict(new { message = result.Message });

            return Ok(result);
        }

        /// PUT /api/v1/finance-project/{projectId}
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int projectId, [FromBody] SaveProjectRequestDto request)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");
            if (string.IsNullOrWhiteSpace(request.ProjectTitle))
                return BadRequest("projectTitle is required.");

            var result = await _repo.SaveProjectAsync("Update", projectId, request);

            if (!result.Success)
            {
                if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { message = result.Message });
                return Conflict(new { message = result.Message });
            }

            return Ok(result);
        }
    }
}
