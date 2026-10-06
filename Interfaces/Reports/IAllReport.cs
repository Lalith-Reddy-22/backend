using Medtrix_PmTool_V2_Api.DTOs.Reports;

namespace Medtrix_PmTool_V2_Api.Interfaces.Reports
{
    public interface IAllReport
    {
        Task<IEnumerable<ProjectDropdownDto>> GetProjectsDropdownAsync();
        Task<ReportPagedResult<TaskReportRowDto>> GetTaskListAsync(TaskListRequestDto request);
        Task<ReportPagedResult<ReportEmployeeDto>> GetEmployeeListAsync(EmployeeListRequestDto request);
        Task<ReportPagedResult<ReportProjectDto>> GetProjectListAsync(ProjectListRequestDto request);
        Task<IEnumerable<EmpTaskModalRowDto>> GetEmpTaskDetailsAsync(EmpTaskDetailModalRequestDto request);
        Task<IEnumerable<ProjectTaskDetailRowDto>> GetProjectTaskDetailsAsync(ProjectTaskDetailRequestDto request);
    }
}
