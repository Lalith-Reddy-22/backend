using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Finance
{
    [ApiController]
    [Route("api/v1/finance-project-list-rate-card")]
    public class FinanceProjectListRateCardController : ControllerBase
    {
        private readonly IFinanceProjectListRateCard _repository;

        public FinanceProjectListRateCardController(
            IFinanceProjectListRateCard repository)
        {
            _repository = repository;
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects()
        {
            var result = await _repository.GetProjectsAsync();

            return Ok(result);
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await _repository.GetEmployeesAsync();

            return Ok(result);
        }

        [HttpGet("buying-center/{projectId}")]
        public async Task<IActionResult> GetBuyingCenter(int projectId)
        {
            var result =
                await _repository.GetBuyingCenterByProjectAsync(projectId);

            return Ok(result);
        }

        // ============================================
        // CREATE
        // POST /api/v1/finance-project-list-rate-card
        // ============================================
        [HttpPost]
        public async Task<IActionResult> CreateRateCard(
            CreateProjectRateCardRequestDto request)
        {
            var result =
                await _repository.CreateProjectRateCardAsync(request);

            return Ok(new
            {
                message = result
            });
        }

        // ============================================
        // EDIT
        // PUT /api/v1/finance-project-list-rate-card/edit
        // ============================================
        [HttpPut("edit")]
        public async Task<IActionResult> UpdateRateCard(
            CreateProjectRateCardRequestDto request)
        {
            var result =
                await _repository.UpdateProjectRateCardAsync(request);

            return Ok(new
            {
                message = result
            });
        }
    }
}