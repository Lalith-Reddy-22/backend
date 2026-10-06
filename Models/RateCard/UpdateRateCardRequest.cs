namespace Medtrix_PmTool_V2_Api.Models.RateCard
{
    public class UpdateRateCardRequest
    {
        public List<RateCardItem> BuyingCenters { get; set; } = new();
    }
}