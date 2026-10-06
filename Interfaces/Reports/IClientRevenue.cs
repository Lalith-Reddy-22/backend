using Medtrix_PmTool_V2_Api.DTOs.Reports;

namespace Medtrix_PmTool_V2_Api.Interfaces.Reports
{
    public interface IClientRevenue
    {
        Task<IEnumerable<ClientRevenueRowDto>> GetClientWisePlannedRevenueAsync(ClientRevenueRequestDto request);
    }
}
