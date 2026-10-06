using Medtrix_PmTool_V2_Api.DTOs.Leave;
using Medtrix_PmTool_V2_Api.DTOs.Reports;

namespace Medtrix_PmTool_V2_Api.Interfaces.Leave
{
    public interface ILeave
    {
        Task<IEnumerable<TeamDropdownDto>> GetTeamsDropdownAsync();
        Task<IEnumerable<DesignationDropdownDto>> GetDesignationsByTeamAsync(int teamId);
        Task<PagedResult<LeaveListDto>> GetAllLeavesAsync(LeaveListRequestDto request);
        Task<LeaveDetailDto?> GetLeaveByIdAsync(int leaveId);
        Task<string> CreateLeaveAsync(CreateLeaveRequestDto request);
        Task<string> UpdateLeaveAsync(int leaveId, UpdateLeaveRequestDto request);
    }
}
