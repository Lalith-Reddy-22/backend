using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Reports
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IDailyReports _dailyReports;
        private readonly IWeekUtilization _weekUtilization;
        private readonly ITeamUtilization _teamUtilization;
        private readonly IClientRevenue _clientRevenue;
        private readonly IAllReport _allReport;

        public ReportsController(IDailyReports dailyReports, IWeekUtilization weekUtilization, ITeamUtilization teamUtilization, IClientRevenue clientRevenue, IAllReport allReport)
        {
            _dailyReports = dailyReports;
            _weekUtilization = weekUtilization;
            _teamUtilization = teamUtilization;
            _clientRevenue = clientRevenue;
            _allReport = allReport;
        }

        [HttpGet("daily-reports")]
        public async Task<IActionResult> GetDailyTaskDetails(
            [FromQuery] DateTime? todayDate,
            [FromQuery] int? taskId,
            [FromQuery] string? resourceName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var request = new DailyReportRequestDto
            {
                TodayDate = todayDate ?? DateTime.Today,
                ResourceName = resourceName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _dailyReports.GetDailyTaskDetailsAsync(request);
            return Ok(result);
        }

        [HttpGet("week-utilization-designation")]
        public async Task<IActionResult> GetWeekUtilization(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("startDate must be before or equal to endDate.");

            var request = new WeekUtilizationRequestDto { StartDate = startDate, EndDate = endDate };
            var result = await _weekUtilization.GetUtilizationSummaryAsync(request);
            return Ok(result);
        }

        [HttpGet("week-utilization-team")]
        public async Task<IActionResult> GetTeamUtilization(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("startDate must be before or equal to endDate.");

            var request = new TeamUtilizationRequestDto { StartDate = startDate, EndDate = endDate };
            var result = await _teamUtilization.GetTeamUtilizationSummaryAsync(request);
            return Ok(result);
        }

        [HttpGet("client-wise-revenue")]
        public async Task<IActionResult> GetClientWisePlannedRevenue(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("startDate must be before or equal to endDate.");

            var request = new ClientRevenueRequestDto { StartDate = startDate, EndDate = endDate };
            var result = await _clientRevenue.GetClientWisePlannedRevenueAsync(request);
            return Ok(result);
        }

        // API 1 — Projects dropdown
        // GET /api/reports/projects-dropdown
        [HttpGet("projects-dropdown")]
        public async Task<IActionResult> GetProjectsDropdown()
        {
            var result = await _allReport.GetProjectsDropdownAsync();
            return Ok(result);
        }

        // API 2 — Task list (Tab 1)
        // GET /api/reports/task-list?empId=5&empType=1&startDate=2026-07-07&endDate=2026-07-11&projectId=0&searchText=PROJ001&pageNumber=1&pageSize=10
        [HttpGet("task-list")]
        public async Task<IActionResult> GetTaskList(
            [FromQuery] int empId,
            [FromQuery] int empType,
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] int projectId = 0,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var request = new TaskListRequestDto
            {
                EmpId = empId, EmpType = empType,
                StartDate = startDate, EndDate = endDate,
                ProjectId = projectId, SearchText = searchText,
                PageNumber = pageNumber, PageSize = pageSize
            };
            var result = await _allReport.GetTaskListAsync(request);
            return Ok(result);
        }

        // API 3 — Employee list (Tab 2)
        // GET /api/reports/employee-list?startDate=2026-07-07&endDate=2026-07-11&searchText=John&pageNumber=1&pageSize=10
        [HttpGet("employee-list")]
        public async Task<IActionResult> GetEmployeeList(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var request = new EmployeeListRequestDto
            {
                StartDate = startDate, EndDate = endDate,
                SearchText = searchText, PageNumber = pageNumber, PageSize = pageSize
            };
            var result = await _allReport.GetEmployeeListAsync(request);
            return Ok(result);
        }

        // API 4 — All project list (Tab 3)
        // GET /api/reports/project-list?empId=5&empType=1&searchText=PTC&pageNumber=1&pageSize=10
        [HttpGet("project-list")]
        public async Task<IActionResult> GetProjectList(
            [FromQuery] int? empId,
            [FromQuery] int empType,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var request = new ProjectListRequestDto
            {
                EmpId = empId, EmpType = empType,
                SearchText = searchText, PageNumber = pageNumber, PageSize = pageSize
            };
            var result = await _allReport.GetProjectListAsync(request);
            return Ok(result);
        }

        // API 5 — Employee task details modal popup
        // GET /api/reports/emp-task-details?employeeId=5&startDate=2026-07-07&endDate=2026-07-11
        [HttpGet("emp-task-details")]
        public async Task<IActionResult> GetEmpTaskDetails(
            [FromQuery] int employeeId,
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            if (employeeId <= 0) return BadRequest("employeeId is required.");
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var request = new EmpTaskDetailModalRequestDto { EmployeeId = employeeId, StartDate = startDate, EndDate = endDate };
            var result = await _allReport.GetEmpTaskDetailsAsync(request);
            return Ok(result);
        }

        // API 6 — Project task details modal popup
        // GET /api/reports/project-task-details?projectId=12&startDate=2026-07-07&endDate=2026-07-11
        [HttpGet("project-task-details")]
        public async Task<IActionResult> GetProjectTaskDetails(
            [FromQuery] int projectId,
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var request = new ProjectTaskDetailRequestDto { ProjectId = projectId, StartDate = startDate, EndDate = endDate };
            var result = await _allReport.GetProjectTaskDetailsAsync(request);
            return Ok(result);
        }
    }
}
