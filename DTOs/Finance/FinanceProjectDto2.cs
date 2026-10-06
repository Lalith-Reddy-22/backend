namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    // ─── Raw SP result DTOs ───────────────────────────────────────────────────

    public class BaseProjectRawDto
    {
        public int PROJECT_ID { get; set; }
        public string projectCode { get; set; } = string.Empty;
        public string project_Title { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string? project_desc { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime? Created_Date { get; set; }
        public int Project_Status { get; set; }
        public double Total_Value { get; set; }
        public double Outsourcing_Value { get; set; }
        public double Vendor_Value { get; set; }
        public int CLIENT_ID { get; set; }
        public int? B_Id { get; set; }
        public string? Client_Title { get; set; }
        public string? B_Name { get; set; }
        public int? Financial_Year_Id { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class EmployeeValueRawDto
    {
        public int project_id { get; set; }
        public int Emp_Id { get; set; }
        public double Working_hours { get; set; }
        public int Task_ID { get; set; }
        public DateTime Start_Date { get; set; }
        public int? B_Id { get; set; }
        public double Emp_Value { get; set; }
    }

    public class ActualLogRawDto
    {
        public int project_id { get; set; }
        public int Emp_Id { get; set; }
        public double Log_hours { get; set; }
        public int Task_ID { get; set; }
        public DateTime Start_Date { get; set; }
        public int? B_Id { get; set; }
        public double Emp_Value { get; set; }
    }

    public class TaskSummaryRawDto
    {
        public int ProjectId { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OpenTasks { get; set; }
    }

    // ─── Final computed response ──────────────────────────────────────────────

    public class ProjectListItemDto2
    {
        public int PROJECT_ID { get; set; }
        public string projectCode { get; set; } = string.Empty;
        public string project_Title { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string? project_desc { get; set; }
        public string? Pstart_date { get; set; }
        public string? end_date { get; set; }
        public string? Created_Date { get; set; }
        public int Project_Status { get; set; }
        public string? Client_Title { get; set; }
        public string? B_Name { get; set; }
        public double Total_Value { get; set; }
        public double Outsourcing_Value { get; set; }
        public double Vendor_Value { get; set; }
        public double Predicted_GP { get; set; }
        public double Predicted_percentage { get; set; }
        public double Actual_GP { get; set; }
        public double Actual_percentage { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OpenTasks { get; set; }
    }

    public class ProjectListResponse2Dto
    {
        public int EmpId { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<ProjectListItemDto2> Projects { get; set; } = Enumerable.Empty<ProjectListItemDto2>();
    }

    // ─── Request DTO ──────────────────────────────────────────────────────────

    public class ProjectListRequestDto
    {
        public int EmpId { get; set; }
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
