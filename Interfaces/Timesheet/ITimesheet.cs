using Medtrix_PmTool_V2_Api.DTOs.Task;
using Medtrix_PmTool_V2_Api.DTOs.Timesheet;

namespace Medtrix_PmTool_V2_Api.Interfaces.Timesheet
{
    public interface ITimesheet
    {
        Task<TimesheetResponseDto> GetTimesheetAsync(TimesheetRequestDto request);
        Task<string> AddTimeLogAsync(AddTimeLogRequestDto request);
        Task<string> RescheduleTaskAsync(RescheduleTaskRequestDto request);
    }
}
