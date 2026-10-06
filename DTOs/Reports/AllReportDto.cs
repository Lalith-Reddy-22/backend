using Medtrix_PmTool_V2_Api.DTOs.Reports;

namespace Medtrix_PmTool_V2_Api.DTOs.Reports
{
    // ─── API 1: Projects dropdown ───────────────────────────────────────────
    public class ProjectDropdownDto
    {
        public int project_ID { get; set; }
        public string project_Title { get; set; } = string.Empty;
    }

    // ─── API 2: Task list — request ─────────────────────────────────────────
    public class TaskListRequestDto
    {
        public int EmpId { get; set; }
        public int EmpType { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int ProjectId { get; set; } = 0;
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ─── API 2: Task list — row ─────────────────────────────────────────────
    public class TaskReportRowDto
    {
        public int Emp_ID { get; set; }
        public int Task_ID { get; set; }
        public string project_Title { get; set; } = string.Empty;
        public int B_Id { get; set; }
        public string projectCode { get; set; } = string.Empty;
        public string Client_Title { get; set; } = string.Empty;
        public string Sub_Client_Name { get; set; } = string.Empty;
        public string Teams_Name { get; set; } = string.Empty;
        public string Task_Title { get; set; } = string.Empty;
        public int Billing_type { get; set; }
        public decimal EmpValue { get; set; }
        public decimal planneHours { get; set; }
        public decimal PlannedValue { get; set; }
        public decimal Planned_Cost { get; set; }
        public decimal Actual_Cost { get; set; }
        public decimal logHours { get; set; }
        public decimal ActualValue { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
        public string EmpDesignation { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    // ─── API 3: Employee list — request ─────────────────────────────────────
    public class EmployeeListRequestDto
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ─── API 3: Employee list — row ─────────────────────────────────────────
    public class ReportEmployeeDto
    {
        public int Emp_ID { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
    }

    // ─── API 4: Project list — request ──────────────────────────────────────
    public class ProjectListRequestDto
    {
        public int? EmpId { get; set; }
        public int EmpType { get; set; }
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ─── API 4: Project list — row ──────────────────────────────────────────
    public class ReportProjectDto
    {
        public int project_ID { get; set; }
        public string project_Title { get; set; } = string.Empty;
        public string projectCode { get; set; } = string.Empty;
    }

    // ─── API 6: Project task details (modal popup) ──────────────────────────
    public class ProjectTaskDetailRequestDto
    {
        public int ProjectId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    public class ProjectTaskDetailRowDto
    {
        public int Task_id { get; set; }
        public string Task_Title { get; set; } = string.Empty;
        public string Task_Unique_ID { get; set; } = string.Empty;
        public int project_ID { get; set; }
        public decimal Planned_hours { get; set; }
        public decimal ActualHours { get; set; }
        public decimal DifferenceLog { get; set; }
        public string CLIENT_TITLE { get; set; } = string.Empty;
    }
    public class EmpTaskDetailModalRequestDto
    {
        public int EmployeeId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    // ─── API 5: Employee task details — row ─────────────────────────────────
    public class EmpTaskModalRowDto
    {
        public int Task_ID { get; set; }
        public string Task_Title { get; set; } = string.Empty;
        public string startDate { get; set; } = string.Empty;
        public string endDate { get; set; } = string.Empty;
        public decimal LogHours { get; set; }
        public decimal PlannerdHours { get; set; }
        public string Tasks { get; set; } = string.Empty;
    }

    // ─── SHARED: paged result wrapper ───────────────────────────────────────
    public class ReportPagedResult<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }
}
