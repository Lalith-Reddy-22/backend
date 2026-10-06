using Medtrix_PmTool_V2_Api.DTOs.Leave;
using Medtrix_PmTool_V2_Api.Interfaces.Leave;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Leave
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeave _leaveRepository;

        public LeaveController(ILeave leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        // GET api/leave/teams
        [HttpGet("teams")]
        public async Task<IActionResult> GetTeams()
        {
            var result = await _leaveRepository.GetTeamsDropdownAsync();
            return Ok(result);
        }

        // GET api/leave/designations?teamId=1
        [HttpGet("designations")]
        public async Task<IActionResult> GetDesignations([FromQuery] int teamId)
        {
            if (teamId <= 0)
                return BadRequest("teamId is required.");

            var result = await _leaveRepository.GetDesignationsByTeamAsync(teamId);
            return Ok(result);
        }

        // GET api/leave?teamName=Tech&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllLeaves(
            [FromQuery] string? teamName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var request = new LeaveListRequestDto
            {
                TeamName   = teamName,
                PageNumber = pageNumber,
                PageSize   = pageSize
            };

            var result = await _leaveRepository.GetAllLeavesAsync(request);
            return Ok(result);
        }

        // GET api/leave/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLeaveById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid leave ID.");

            var result = await _leaveRepository.GetLeaveByIdAsync(id);
            if (result == null)
                return NotFound($"Leave with ID {id} not found.");

            return Ok(result);
        }

        // POST api/leave  → Insert
        [HttpPost]
        public async Task<IActionResult> CreateLeave([FromBody] CreateLeaveRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var status = await _leaveRepository.CreateLeaveAsync(request);

            if (status == "Inserted")
                return Ok(new { message = status, success = true });

            return BadRequest(new { message = "Failed to create leave.", success = false });
        }

        // PUT api/leave/5  → Update
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateLeave(int id, [FromBody] UpdateLeaveRequestDto request)
        {
            if (id <= 0)
                return BadRequest("Invalid leave ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var status = await _leaveRepository.UpdateLeaveAsync(id, request);

            if (status == "Updated")
                return Ok(new { message = status, success = true });

            return BadRequest(new { message = "Failed to update leave.", success = false });
        }
    }
}
