using Medtrix_PmTool_V2_Api.DTOs.Finance;

namespace Medtrix_PmTool_V2_Api.Interfaces.Finance
{
    public interface IFinance
    {
        Task<GetClientGpResponseDto> GetClientGpAsync(int? finYrId, string? searchText, int pageNumber, int pageSize);
        Task<IEnumerable<FinancialYearDto>> GetAllFinancialYearsAsync();
        Task<GetClientProjectsResponseDto> GetProjectsByClientAsync(int clientId, int? finYrId, int? projectStatus, string? searchText, int pageNumber, int pageSize);
        Task<ClientProjectsBySpResponseDto> GetClientProjectsBySpAsync(int clientId, int? finYrId);
        Task<ProjectTaskReportResponseDto> GetProjectTaskReportAsync(int projId, string? searchText, int pageNumber, int pageSize);
        Task<string> SaveClientAsync(CreateClientRequestDto request);
        Task<ClientDetailDto?> GetClientByIdAsync(int clientId);
    }
}
