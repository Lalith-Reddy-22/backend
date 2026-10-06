using Medtrix_PmTool_V2_Api.DTOs.ResourceAllocation;
using Medtrix_PmTool_V2_Api.Interfaces.ResourceAllocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.ResourceAllocation
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/resource-allocation")]
    [ApiController]
    public class ResourceAllocationController : ControllerBase
    {
        private readonly IResourceAllocation _resourceAllocation;

        public ResourceAllocationController(IResourceAllocation resourceAllocation)
        {
            _resourceAllocation = resourceAllocation;
        }

        /// GET /api/resource-allocation?startDate=2026-01-02&endDate=2026-01-20&teamId=1
        [HttpGet]
        public async Task<IActionResult> GetResourceAllocation(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] int teamId)
        {
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            if (teamId <= 0)
                return BadRequest("teamId is required.");

            var request = new ResourceAllocationRequestDto
            {
                StartDate = startDate,
                EndDate   = endDate,
                TeamId    = teamId
            };

            var result = await _resourceAllocation.GetResourceAllocationAsync(request);
            return Ok(result);
        }

        /// GET /api/resource-allocation/task-detail?empId=3&dateVal=2024-09-27
        [HttpGet("task-detail")]
        public async Task<IActionResult> GetEmpTaskDetail(
            [FromQuery] int empId,
            [FromQuery] string dateVal)
        {
            if (empId <= 0)
                return BadRequest("empId is required.");

            if (string.IsNullOrWhiteSpace(dateVal))
                return BadRequest("dateVal is required.");

            var request = new EmpTaskDetailRequestDto { EmpId = empId, DateVal = dateVal };
            var result  = await _resourceAllocation.GetEmpTaskDetailAsync(request);
            return Ok(result);
        }
    }
}
