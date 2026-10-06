namespace Medtrix_PmTool_V2_Api.DTOs.Asset
{
    public class AssetItemDto
    {
        public int Id { get; set; }
        public string Product_Name { get; set; }
        public string BrandName { get; set; }
        public string Product_Icon { get; set; }
        public string P_Status { get; set; }
        public string ChkOption { get; set; }
    }

    public class MyAssetRequestDto
    {
        public string Request_Id { get; set; }
        public string ItemList { get; set; }
        public int Emp_Id { get; set; }
        public string EmpName { get; set; } = string.Empty;
        public string Reason { get; set; }
        public string statusName { get; set; }
        public DateTime Request_RaisedDate { get; set; }
        public DateTime? Item_IssueDate { get; set; }
        public DateTime? Item_TakeBackDate { get; set; }
    }

    public class RaiseAssetRequestDto
    {
        public int EmpId { get; set; }
        public string Reason { get; set; }
        public List<int> ProductIds { get; set; }
    }

    public class RaiseAssetResponseDto
    {
        public int RequestId { get; set; }
        public string Message { get; set; }
    }

    public class UpdateAssetStatusRequestDto
    {
        public string ReqId { get; set; }
        public int Status { get; set; }
    }

    public class UpdateAssetStatusResponseDto
    {
        public string Message { get; set; }
    }

    public class AddAssetRequestDto
    {
        public string P_Id { get; set; }
        public string Product_Name { get; set; }
        public string Service_Number { get; set; }
        public string Storage { get; set; }
        public string IsActive { get; set; }
    }
}
