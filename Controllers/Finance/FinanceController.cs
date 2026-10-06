using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Finance
{
    [Authorize(Policy = "FinanceAccess")]
    [Route("api/v1/finance")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IFinance _financeRepository;

        public FinanceController(IFinance financeRepository)
        {
            _financeRepository = financeRepository;
        }

        // GET /api/v1/finance/financial-years
        [HttpGet("financial-years")]
        public async Task<IActionResult> GetAllFinancialYears()
        {
            var result = await _financeRepository.GetAllFinancialYearsAsync();
            return Ok(result);
        }
    }
}
