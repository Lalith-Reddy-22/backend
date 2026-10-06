namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    public class FinancialYearDto
    {
        public int Id { get; set; }
        public string Financial_Year { get; set; }
        public int OrderList { get; set; }
    }

    public class ClientGpItemDto
    {
        public int Client_ID { get; set; }
        public string Client_Title { get; set; }
        public string PoValue { get; set; }
        public string Predicted_Gp { get; set; }
        public double Predicted_percentage { get; set; }
        public string Actual_Gp { get; set; }
        public double Actual_percentage { get; set; }
        public double Total_PO_Value { get; set; }
        public int Project_Count { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class GetClientGpResponseDto
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<ClientGpItemDto> Clients { get; set; }
    }

    public class ClientProjectItemDto
    {
        public string ProjectCode { get; set; }
        public int Project_ID { get; set; }
        public string Client_Title { get; set; }
        public string BName { get; set; }
        public string Financial_Year { get; set; }
        public string Project_Title { get; set; }
        public string Employee_Name { get; set; }
        public string Project_Status { get; set; }
        public int Status_ID { get; set; }
        public string PO_Value { get; set; }
        public string Outsourcing_Cost { get; set; }
        public string Effect_EstimateCost { get; set; }
        public double Predicted_Hours { get; set; }
        public double Actual_Hours { get; set; }
        public string Predicted_Cost { get; set; }
        public string Predicted_GP { get; set; }
        public string Actual_Cost { get; set; }
        public string Actual_GP { get; set; }
        public double Predicted_percentage { get; set; }
        public double Actual_percentage { get; set; }
        public double PO_Value_Numeric { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class GetClientProjectsResponseDto
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<ClientProjectItemDto> Projects { get; set; }
     
    }

    // ─── Client Projects by SP (spGetProjectDetailsByClientID) ───────────────
    public class ClientProjectBySpDto
    {
        public int Project_ID { get; set; }
        public string Project_Title { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string Client_Title { get; set; } = string.Empty;
        public string Project_Status { get; set; } = string.Empty;
        public string Start_Date { get; set; } = string.Empty;
        public string End_Date { get; set; } = string.Empty;
        public string Employee_Name { get; set; } = string.Empty;
        public string Financial_Year { get; set; } = string.Empty;
        public string PO_Value { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }

    public class ClientProjectsBySpResponseDto
    {
        public int ClientId { get; set; }
        public int? FinYrId { get; set; }
        public IEnumerable<ClientProjectBySpDto> Projects { get; set; } = Enumerable.Empty<ClientProjectBySpDto>();
    }

    // ─── Project Task Report (sp_get_project_task_report) ────────────────────
    public class ProjectTaskReportItemDto
    {
        public int Task_ID { get; set; }
        public int Emp_ID { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
        public string Project_Title { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public int B_Id { get; set; }
        public string Client_Title { get; set; } = string.Empty;
        public string Teams_Name { get; set; } = string.Empty;
        public string Task_Title { get; set; } = string.Empty;
        public string Start_Date_Formatted { get; set; } = string.Empty;
        public string End_Date_Formatted { get; set; } = string.Empty;
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public double EmpValue { get; set; }
        public double PlannedHours { get; set; }
        public double PlannedValue { get; set; }
        public double ActualHours { get; set; }
        public double ActualValue { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class ProjectTaskReportResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<ProjectTaskReportItemDto> Tasks { get; set; } = Enumerable.Empty<ProjectTaskReportItemDto>();
    }
}
