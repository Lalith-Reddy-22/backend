namespace Medtrix_PmTool_V2_Api.DTOs.Project
{
    public class ProjectCostInfoResponseDto
    {
        public double TotalValue { get; set; }
        public double PredictedValue { get; set; }
        public double OutsourcingValue { get; set; }
        public double UtilizationCost { get; set; }
    }

    public class GetAllProjectsRequestDto
    {
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ProjectListItemDto
    {
        public int Project_Id { get; set; }
        public int Client_Id { get; set; }
        public string Client_Title { get; set; }
        public string Project_Title { get; set; }
        public string ProjectCode { get; set; }
        public int Financial_Year_Id { get; set; }
        public int B_Id { get; set; }
        public string Financial_Year { get; set; }
        public string ClsClientTitle { get; set; }
        public double TotalValue { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public int Project_Status { get; set; }
        public string Project_Status_Text { get; set; }
        public int EmpCount { get; set; }
        public int Completed_Tasks { get; set; }
        public int Total_Tasks { get; set; }
        public double Logged_Hours { get; set; }
        public double Allocated_Hours { get; set; }

    }

    public class GetAllProjectsResponseDto
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<ProjectListItemDto> Projects { get; set; }
    }

    public class DeleteProjectRequestDto
    {
        public int DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    public class DeleteProjectResponseDto
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectCode { get; set; }
        public int TasksDeleted { get; set; }
        public string Message { get; set; }
    }

    public class ProjectTaskDto
    {
        public int Task_Id { get; set; }
        public string? Task_Title { get; set; }
        public string? Project_Code { get; set; }
        public string? Project_Title { get; set; }
        public int Project_Id { get; set; }
        public int BuyingCenterId { get; set; }
        public string? BuyingCenterName { get; set; }
        public string? Employee_Name { get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
    }
}
