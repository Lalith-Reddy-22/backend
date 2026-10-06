using Medtrix_PmTool_V2_Api.DTOs.Finance;

namespace Medtrix_PmTool_V2_Api.Interfaces.Finance
{
    public interface IFinanceRateCard
    {
        Task<object> GetRateCardByTeamAsync(int teamId);
        Task<IEnumerable<BuyingCenterDetailDto>> GetBuyingCenterDetailsAsync();
        Task<CloneBuyingCenterResponseDto> CloneBuyingCenterAsync(CloneBuyingCenterRequestDto request);
    }
}
