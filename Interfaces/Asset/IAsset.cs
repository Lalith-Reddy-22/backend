using Medtrix_PmTool_V2_Api.DTOs.Asset;

namespace Medtrix_PmTool_V2_Api.Interfaces.Asset
{
    public interface IAsset
    {
        Task<IEnumerable<AssetItemDto>> GetAllAssetsAsync();
        Task<IEnumerable<MyAssetRequestDto>> GetMyRequestsAsync(int empId);
        Task<IEnumerable<MyAssetRequestDto>> GetAllRequestsAsync();
        Task<RaiseAssetResponseDto> RaiseAssetRequestAsync(RaiseAssetRequestDto request);
        Task<UpdateAssetStatusResponseDto> UpdateAssetStatusAsync(UpdateAssetStatusRequestDto request);
        Task<bool> AddAssetAsync(AddAssetRequestDto request);
    }
}
