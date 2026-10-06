namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    public class FinanceProjectListItemDto
    {
        public string projectCode { get; set; } = string.Empty;
        public string project_Title { get; set; } = string.Empty;
        public int PROJECT_ID { get; set; }
        public string Region { get; set; } = string.Empty;
        public string? project_desc { get; set; }
        public string Pstart_date { get; set; } = string.Empty;
        public int Project_Status { get; set; }
        public string end_date { get; set; } = string.Empty;
        public string Created_Date { get; set; } = string.Empty;
        public string Client_Title { get; set; } = string.Empty;
        public string B_Name { get; set; } = string.Empty;
        public string Predicted_GP { get; set; } = string.Empty;
        public double Predicted_percentage { get; set; }
        public string Actual_GP { get; set; } = string.Empty;
        public double Actual_percentage { get; set; }
        public double Outsourcing_Value { get; set; }
        public double Vendor_Value { get; set; }
        public double Total_Value { get; set; }
    }

    public class FinanceProjectListResponseDto
    {
        public int EmpId { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<FinanceProjectListItemDto> Projects { get; set; } = Enumerable.Empty<FinanceProjectListItemDto>();
    }
}
