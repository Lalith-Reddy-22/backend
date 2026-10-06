namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    public class BuyingCenterResponseDto
    {
        public string BuyingCenterName { get; set; } = string.Empty;

        public List<BuyingCenterEmployeeDto> Employees { get; set; } = new();
    }
}