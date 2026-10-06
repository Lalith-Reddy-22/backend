using Medtrix_PmTool_V2_Api.DTOs.Project;

namespace Medtrix_PmTool_V2_Api.Interfaces.Project
{
    public interface IProject
    {
        Task<ProjectCostInfoResponseDto> GetProjectCostInfoAsync(int projectId, int buyingCenterId);
        Task<GetAllProjectsResponseDto> GetAllProjectsAsync(GetAllProjectsRequestDto request);
        Task<DeleteProjectResponseDto> DeleteProjectAsync(int projectId, DeleteProjectRequestDto request);
        Task<IEnumerable<ProjectTaskDto>> GetProjectTasksAsync(int projectId, int empId, int userType);
    }
}
