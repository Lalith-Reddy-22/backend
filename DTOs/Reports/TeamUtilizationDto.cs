namespace Medtrix_PmTool_V2_Api.DTOs.Reports
{
    public class TeamUtilizationRowDto
    {
        public string SummaryType { get; set; } = string.Empty;
        public string Teams_Name { get; set; } = string.Empty;
        public decimal Plan_Hours { get; set; }
        public decimal Availability_Hrs { get; set; }
        public string Utilization_Plan { get; set; } = string.Empty;
        public decimal Planned_Rev_Rec { get; set; }
    }

    public class TeamUtilizationRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class TeamUtilizationResponseDto
    {
        public IEnumerable<TeamUtilizationRowDto> Billable { get; set; } = Enumerable.Empty<TeamUtilizationRowDto>();
        public IEnumerable<TeamUtilizationRowDto> NonBillable { get; set; } = Enumerable.Empty<TeamUtilizationRowDto>();
    }
}
