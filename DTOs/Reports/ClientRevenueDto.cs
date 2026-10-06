namespace Medtrix_PmTool_V2_Api.DTOs.Reports
{
    public class ClientRevenueRowDto
    {
        public string ClientwiseRR { get; set; } = string.Empty;
        public double Planned_Hrs { get; set; }
        public double Planned_Cost { get; set; }
        public string Utilization { get; set; } = string.Empty;
    }

    public class ClientRevenueRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
