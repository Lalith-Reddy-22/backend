using Medtrix_PmTool_V2_Api.DTOs.Finance;

namespace Medtrix_PmTool_V2_Api.Interfaces.Finance
{
    public interface IFinanceProjectList2
    {
        Task<ProjectListResponse2Dto> GetProjectListAsync(ProjectListRequestDto request);
    }
}
