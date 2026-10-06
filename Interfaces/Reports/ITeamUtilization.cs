using Medtrix_PmTool_V2_Api.DTOs.Reports;

namespace Medtrix_PmTool_V2_Api.Interfaces.Reports
{
    public interface ITeamUtilization
    {
        Task<TeamUtilizationResponseDto> GetTeamUtilizationSummaryAsync(TeamUtilizationRequestDto request);
    }
}
