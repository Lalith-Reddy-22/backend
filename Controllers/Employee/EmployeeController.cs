using Medtrix_PmTool_V2_Api.DTOs.Employee;
using Medtrix_PmTool_V2_Api.Interfaces.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Employees
{
    //[Authorize(Policy = "FinanceAccess")]
    [Route("api/v1/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employeeRepository;

        public EmployeeController(IEmployee employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET /api/v1/employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET /api/v1/employee/names
        [HttpGet("names")]
        public async Task<IActionResult> GetAllEmployeeNamesAndIds()
        {
            var result = await _employeeRepository.GetAllEmployeeNameAndIdAsync();
            return Ok(result);
        }

        // GET /api/v1/employee/types
        [HttpGet("types")]
        public async Task<IActionResult> GetEmpTypes()
        {
            var result = await _employeeRepository.GetEmpTypesAsync();
            return Ok(result);
        }

        // GET /api/v1/employee/sub-teams?teamId=2
        [HttpGet("sub-teams")]
        public async Task<IActionResult> GetSubTeams([FromQuery] int teamId)
        {
            if (teamId <= 0) return BadRequest("teamId is required.");
            var result = await _employeeRepository.GetSubTeamsAsync(teamId);
            return Ok(result);
        }

        // GET /api/v1/employee/project-details?empId=5&searchText=pfizer&pageNumber=1&pageSize=10
        [HttpGet("project-details")]
        public async Task<IActionResult> GetEmployeeProjectDetails(
            [FromQuery] int empId,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            var result = await _employeeRepository.GetEmployeeProjectDetailsAsync(empId, searchText, pageNumber, pageSize);
            return Ok(result);
        }

        // GET /api/v1/employee/details?empId=5
        [HttpGet("details")]
        public async Task<IActionResult> GetEmployeeDetailsById([FromQuery] int empId)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            var result = await _employeeRepository.GetEmployeeDetailsByIdAsync(empId);
            if (result.EmployeeInfo == null) return NotFound("Employee not found.");
            return Ok(result);
        }

        // POST /api/v1/employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(request.EmailId))
                return BadRequest("EmailId is required.");

            var result = await _employeeRepository.CreateEmployeeAsync(request);

            if (!result.Success)
                return Conflict(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // POST /api/v1/employee/hash-existing-passwords
        [HttpPost("hash-existing-passwords")]
        public async Task<IActionResult> HashExistingPasswords()
        {
            var result = await _employeeRepository.HashExistingPasswordsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT /api/v1/employee/{empId}
        [HttpPut("{empId:int}")]
        public async Task<IActionResult> UpdateEmployee(int empId, [FromBody] UpdateEmployeeRequestDto request)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            if (string.IsNullOrWhiteSpace(request.EmpName)) return BadRequest("EmpName is required.");
            if (string.IsNullOrWhiteSpace(request.Email))   return BadRequest("Email is required.");

            var result = await _employeeRepository.UpdateEmployeeAsync(empId, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}
