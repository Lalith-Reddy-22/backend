using Medtrix_PmTool_V2_Api.DTOs.Asset;
using Medtrix_PmTool_V2_Api.Interfaces.Asset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Asset
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/asset")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAsset _assetRepository;

        public AssetController(IAsset assetRepository)
        {
            _assetRepository = assetRepository;
        }

        // GET /api/v1/asset/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAssets()
        {
            var result = await _assetRepository.GetAllAssetsAsync();
            return Ok(result);
        }

        // GET /api/v1/asset/my-requests?empId=5
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] int empId)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            var result = await _assetRepository.GetMyRequestsAsync(empId);
            return Ok(result);
        }

        // GET /api/v1/asset/all-requests  (system admin)
        [HttpGet("all-requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var result = await _assetRepository.GetAllRequestsAsync();
            return Ok(result);
        }

        // POST /api/v1/asset/raise-request
        [HttpPost("raise-request")]
        public async Task<IActionResult> RaiseAssetRequest([FromBody] RaiseAssetRequestDto request)
        {
            if (request.EmpId <= 0) return BadRequest("empId is required.");
            if (string.IsNullOrWhiteSpace(request.Reason)) return BadRequest("Reason is required.");
            if (request.ProductIds == null || request.ProductIds.Count == 0) return BadRequest("At least one product must be selected.");

            var result = await _assetRepository.RaiseAssetRequestAsync(request);
            return Ok(result);
        }

        // PUT /api/v1/asset/update-status  (system admin only)
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateAssetStatus([FromBody] UpdateAssetStatusRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ReqId)) return BadRequest("reqId is required.");
            if (request.Status <= 0) return BadRequest("status is required.");

            var result = await _assetRepository.UpdateAssetStatusAsync(request);
            return Ok(result);
        }

        // POST /api/v1/asset/add-asset  (system admin only)
        [HttpPost("add-asset")]
        public async Task<IActionResult> AddAsset([FromBody] AddAssetRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.P_Id)) return BadRequest("P_Id is required.");
            if (string.IsNullOrWhiteSpace(request.Product_Name)) return BadRequest("Product_Name is required.");

            var result = await _assetRepository.AddAssetAsync(request);
            return result ? Ok(new { Message = "Asset added successfully." }) : StatusCode(500, new { Message = "Failed to add asset." });
        }
    }
}
