using Medtrix_PmTool_V2_Api.DTOs.Reports;

namespace Medtrix_PmTool_V2_Api.Interfaces.Reports
{
    public interface IWeekUtilization
    {
        Task<WeekUtilizationResponseDto> GetUtilizationSummaryAsync(WeekUtilizationRequestDto request);
    }
}
