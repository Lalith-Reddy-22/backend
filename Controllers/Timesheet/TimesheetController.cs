using Medtrix_PmTool_V2_Api.DTOs.Task;
using Medtrix_PmTool_V2_Api.DTOs.Timesheet;
using Medtrix_PmTool_V2_Api.Interfaces.Timesheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Medtrix_PmTool_V2_Api.DTOs.Task;

namespace Medtrix_PmTool_V2_Api.Controllers.Timesheet
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheet _timesheet;

        public TimesheetController(ITimesheet timesheet)
        {
            _timesheet = timesheet;
        }

        /// GET /api/timesheet?empId=3&startDate=2024-01-08&endDate=2024-01-12
        [HttpGet]
        public async Task<IActionResult> GetTimesheet(
            [FromQuery] int empId,
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            if (empId <= 0)
                return BadRequest("empId is required.");

            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var request = new TimesheetRequestDto
            {
                EmpId     = empId,
                StartDate = startDate,
                EndDate   = endDate
            };

            var result = await _timesheet.GetTimesheetAsync(request);
            return Ok(result);
        }

        /// POST /api/timesheet/log
        /// Body: { timeLogId, taskId, empId, logDate, logHours, notes }
        /// timeLogId = 0 → insert,  timeLogId > 0 → update
        [HttpPost("log")]
        public async Task<IActionResult> AddTimeLog([FromBody] AddTimeLogRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.TaskId <= 0) return BadRequest("taskId is required.");
            if (request.EmpId <= 0)  return BadRequest("empId is required.");
            if (string.IsNullOrWhiteSpace(request.LogDate)) return BadRequest("logDate is required.");
            if (request.LogHours < 0) return BadRequest("Invalid Log Hours.");

            var msg = await _timesheet.AddTimeLogAsync(request);
            return Ok(new { message = msg, success = true });
        }

        /// POST /api/timesheet/reschedule
        [HttpPost("reschedule")]
        public async Task<IActionResult> RescheduleTask(
            [FromBody] RescheduleTaskRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.TaskId <= 0)
                return BadRequest("TaskId is required.");

            if (request.EmployeeId <= 0)
                return BadRequest("EmployeeId is required.");

            if (request.OriginalDate == default)
                return BadRequest("OriginalDate is required.");

            if (request.RescheduleDate == default)
                return BadRequest("RescheduleDate is required.");

            if (string.IsNullOrWhiteSpace(request.RescheduleType))
                return BadRequest("RescheduleType is required.");

            var result = await _timesheet.RescheduleTaskAsync(request);

            return Ok(result);
        }
    }
}
