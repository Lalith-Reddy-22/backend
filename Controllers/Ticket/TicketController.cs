using Medtrix_PmTool_V2_Api.DTOs.Ticket;
using Medtrix_PmTool_V2_Api.Interfaces.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Ticket
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/ticket")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicket _ticketRepository;

        public TicketController(ITicket ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // GET /api/v1/ticket/list?empId=5&empType=1&startDate=&endDate=&searchText=&statusId=&severityId=&issueTypeId=&pageNumber=1&pageSize=10
        [HttpGet("list")]
        public async Task<IActionResult> GetTicketList(
            [FromQuery] int empId,
            [FromQuery] int empType = 1,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? searchText = null,
            [FromQuery] int? statusId = null,
            [FromQuery] int? severityId = null,
            [FromQuery] int? issueTypeId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (empId <= 0) return BadRequest("empId is required.");

            var result = await _ticketRepository.GetTicketListAsync(new GetTicketListRequestDto
            {
                EmpId       = empId,
                EmpType     = empType,
                StartDate   = startDate,
                EndDate     = endDate,
                SearchText  = searchText,
                StatusId    = statusId,
                SeverityId  = severityId,
                IssueTypeId = issueTypeId,
                PageNumber  = pageNumber,
                PageSize    = pageSize
            });
            return Ok(result);
        }

        // GET /api/v1/ticket/form-data
        [HttpGet("form-data")]
        public async Task<IActionResult> GetFormData()
        {
            var result = await _ticketRepository.GetFormDataAsync();
            return Ok(result);
        }

        // POST /api/v1/ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequestDto request)
        {
            if (request.EmpId <= 0)                              return BadRequest("empId is required.");
            if (request.IssueType <= 0)                          return BadRequest("issueType is required.");
            if (request.Severity <= 0)                           return BadRequest("severity is required.");
            if (string.IsNullOrWhiteSpace(request.IssueDescription)) return BadRequest("issueDescription is required.");

            var result = await _ticketRepository.CreateTicketAsync(request);
            return Ok(result);
        }

        // PUT /api/v1/ticket/update-status
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketStatusRequestDto request)
        {
            if (request.IssueId <= 0)    return BadRequest("issueId is required.");
            if (request.UpdatedBy <= 0)  return BadRequest("updatedBy is required.");

            var result = await _ticketRepository.UpdateTicketStatusAsync(request);

            if (result.Issue_Id == 0 && result.Message == "Issue not found.")
                return NotFound(new { result.Message });

            return Ok(result);
        }
    }
}
