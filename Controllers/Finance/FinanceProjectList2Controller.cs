using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Finance
{
    [Authorize(Policy = "FinanceAccess")]
    [Route("api/v1/finance-projects")]
    [ApiController]
    public class FinanceProjectList2Controller : ControllerBase
    {
        private readonly IFinanceProjectList2 _repo;

        public FinanceProjectList2Controller(IFinanceProjectList2 repo)
        {
            _repo = repo;
        }

        /// GET /api/v1/finance-projects?empId=10&pageNumber=1&pageSize=10
        /// Optional filters: searchText, clientId, financialYearId, projectStatus
        [HttpGet]
        public async Task<IActionResult> GetProjectList([FromQuery] ProjectListRequestDto request)
        {
            if (request.EmpId <= 0) return BadRequest("empId is required.");

            var result = await _repo.GetProjectListAsync(request);
            return Ok(result);
        }
    }
}
