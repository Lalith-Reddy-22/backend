namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    public class CreateProjectRateCardRequestDto
    {
        public string Buying_CenterName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public List<ProjectRateCardEmployeeDto> EmpSets { get; set; } = new();
    }
}