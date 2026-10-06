namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    // ─── Buying Center Details (Sp_GetBuyingCenterDetails) ───────────────────
    public class BuyingCenterDetailDto
    {
        public int BId { get; set; }
        public string BName { get; set; } = string.Empty;
        public string Created_Date { get; set; } = string.Empty;
        public int IsActive { get; set; }
        public string Created_By { get; set; } = string.Empty;
    }

    // ─── Clone Buying Center Request ─────────────────────────────────────────
    public class CloneBuyingCenterRequestDto
    {
        public int OrgBId { get; set; }
        public string BuyCenterName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
    }

    public class CloneBuyingCenterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
