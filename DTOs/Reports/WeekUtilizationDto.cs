namespace Medtrix_PmTool_V2_Api.DTOs.Reports
{
    public class UtilizationRowDto
    {
        public string Type { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string EmpDesignation { get; set; } = string.Empty;
        public int NumEmployees { get; set; }
        public decimal Plan_Hours { get; set; }
        public decimal Available_Hours { get; set; }
        public string Utilization { get; set; } = string.Empty;
    }

    public class WeekUtilizationRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class WeekUtilizationResponseDto
    {
        public IEnumerable<UtilizationRowDto> Billable { get; set; } = Enumerable.Empty<UtilizationRowDto>();
        public IEnumerable<UtilizationRowDto> NonBillable { get; set; } = Enumerable.Empty<UtilizationRowDto>();
    }
}
