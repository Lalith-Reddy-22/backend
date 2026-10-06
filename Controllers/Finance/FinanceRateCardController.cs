using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Finance
{
    [Authorize(Policy = "FinanceAccess")]
    [Route("api/v1/finance-rate-card")]
    [ApiController]
    public class FinanceRateCardController : ControllerBase
    {
        private readonly IFinanceRateCard _repo;

        public FinanceRateCardController(IFinanceRateCard repo)
        {
            _repo = repo;
        }

        /// GET /api/v1/finance-rate-card?teamId=1
        /// teamId: 1=Tech, 2=Creative, 3=Content, 4/5/6=PM
        /// Returns dynamic columns based on BuyingCenter names for the team
        [HttpGet]
        public async Task<IActionResult> GetRateCard([FromQuery] int teamId)
        {
            if (teamId <= 0) return BadRequest("teamId is required.");

            var result = await _repo.GetRateCardByTeamAsync(teamId);
            return Ok(result);
        }

        /// GET /api/v1/finance-rate-card/buying-centers
        /// Returns all buying center details
        [HttpGet("buying-centers")]
        public async Task<IActionResult> GetBuyingCenterDetails()
        {
            var result = await _repo.GetBuyingCenterDetailsAsync();
            return Ok(result);
        }

        /// POST /api/v1/finance-rate-card/buying-centers/clone
        /// Clones an existing buying center with all employee rate values
        /// Body: { "orgBId": 8, "buyCenterName": "US-East", "createdBy": 128 }
        [HttpPost("buying-centers/clone")]
        public async Task<IActionResult> CloneBuyingCenter([FromBody] DTOs.Finance.CloneBuyingCenterRequestDto request)
        {
            if (request.OrgBId <= 0)
                return BadRequest("orgBId is required.");
            if (string.IsNullOrWhiteSpace(request.BuyCenterName))
                return BadRequest("buyCenterName is required.");
            if (request.CreatedBy <= 0)
                return BadRequest("createdBy is required.");

            var result = await _repo.CloneBuyingCenterAsync(request);
            return Ok(result);
        }
    }
}
