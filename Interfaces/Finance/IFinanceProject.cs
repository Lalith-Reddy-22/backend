using Medtrix_PmTool_V2_Api.DTOs.Finance;

namespace Medtrix_PmTool_V2_Api.Interfaces.Finance
{
    public interface IFinanceProject
    {
        // All dropdowns in one SP call
        Task<ProjectLookupsResponseDto> GetProjectLookupsAsync();

        // Project CRUD
        Task<FinanceProjectDetailDto?> GetProjectByIdAsync(int projectId);
        Task<SaveProjectResponseDto> SaveProjectAsync(string action, int? projectId, SaveProjectRequestDto request);
    }
}
