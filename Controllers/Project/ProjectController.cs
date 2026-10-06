using Medtrix_PmTool_V2_Api.DTOs.Project;
using Medtrix_PmTool_V2_Api.Interfaces.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Project
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProject _projectRepository;

        public ProjectController(IProject projectRepository)
        {
            _projectRepository = projectRepository;
        }

        // GET /api/v1/project/all?searchText=&pageNumber=1&pageSize=10
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProjects(
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _projectRepository.GetAllProjectsAsync(new GetAllProjectsRequestDto
            {
                SearchText = searchText,
                PageNumber = pageNumber,
                PageSize   = pageSize
            });
            return Ok(result);
        }

        // GET /api/v1/project/cost-info?projectId=5636&buyingCenterId=8
        [HttpGet("cost-info")]
        public async Task<IActionResult> GetProjectCostInfo(
            [FromQuery] int projectId,
            [FromQuery] int buyingCenterId = 0)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");
            var result = await _projectRepository.GetProjectCostInfoAsync(projectId, buyingCenterId);
            return Ok(result);
        }
        // DELETE /api/v1/project/{projectId}
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int projectId, [FromBody] DeleteProjectRequestDto request)
        {
            if (projectId <= 0)        return BadRequest("projectId is required.");
            if (request.DeletedBy <= 0) return BadRequest("deletedBy is required.");

            var result = await _projectRepository.DeleteProjectAsync(projectId, request);

            if (result.ProjectId == 0)
                return NotFound(new { result.Message });

            return Ok(result);
        }

        // GET /api/v1/project/{projectId}/tasks
        [HttpGet("{projectId}/tasks")]
        public async Task<IActionResult> GetProjectTasks(
            [FromRoute] int projectId)
        {
            if (projectId <= 0)
                return BadRequest("projectId is required.");

            try
            {
                // Get employee ID from JWT
                int empId = 0;
                int userType = 0;

                var empIdClaim = User.FindFirst("empId")?.Value;
                var userTypeClaim = User.FindFirst("userType")?.Value;

                if (!string.IsNullOrEmpty(empIdClaim))
                {
                    int.TryParse(empIdClaim, out empId);
                }

                if (!string.IsNullOrEmpty(userTypeClaim))
                {
                    int.TryParse(userTypeClaim, out userType);
                }

                var result = await _projectRepository.GetProjectTasksAsync(
                    projectId,
                    empId,
                    userType);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to get project tasks.",
                    error = ex.Message
                });
            }
        }
    }
}
