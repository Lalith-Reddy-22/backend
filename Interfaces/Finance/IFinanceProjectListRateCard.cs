using Medtrix_PmTool_V2_Api.DTOs.Finance;

namespace Medtrix_PmTool_V2_Api.Interfaces.Finance
{
    public interface IFinanceProjectListRateCard
    {
        Task<List<ProjectDropdownDto>> GetProjectsAsync();

        Task<List<EmployeeDropDownDto>> GetEmployeesAsync();

        Task<BuyingCenterResponseDto> GetBuyingCenterByProjectAsync(int projectId);
        Task<string> CreateProjectRateCardAsync(CreateProjectRateCardRequestDto request);

        Task<string> UpdateProjectRateCardAsync(CreateProjectRateCardRequestDto request);
    }
}