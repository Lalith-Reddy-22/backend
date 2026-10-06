using Dapper;
using Medtrix_PmTool_V2_Api.DbContext; // Ensure this is the correct namespace for your DbContext class
using Medtrix_PmTool_V2_Api.DTOs.Asset;
using Medtrix_PmTool_V2_Api.Interfaces.Asset;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Asset
{
    public class AssetRepository : IAsset
    {
        private readonly DbContext.DbContext _db;

        public AssetRepository(DbContext.DbContext db) 
        {
            _db = db;
        }

        public async Task<IEnumerable<AssetItemDto>> GetAllAssetsAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<AssetItemDto>("sp_getallassestsdetails", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MyAssetRequestDto>> GetMyRequestsAsync(int empId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<MyAssetRequestDto>(
                "Sp_GetAllUsers_AssestsDetails",
                new { Emp_Id = empId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MyAssetRequestDto>> GetAllRequestsAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<MyAssetRequestDto>(
                "Sp_GetAllUsers_AssestsDetails",
                new { Emp_Id = 0 },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RaiseAssetResponseDto> RaiseAssetRequestAsync(RaiseAssetRequestDto request)
        {
            using var conn = _db.CreateConnection();

            var insertParams = new DynamicParameters();
            insertParams.Add("@Emp_Id", request.EmpId);
            insertParams.Add("@Reason", request.Reason);
            insertParams.Add("@RetTicketId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("Sp_InsertAssestsUsersDetail", insertParams, commandType: CommandType.StoredProcedure);

            int ticketId = insertParams.Get<int>("@RetTicketId");

            if (ticketId > 0)
            {
                foreach (var productId in request.ProductIds)
                {
                    await conn.ExecuteAsync(
                        "Sp_InsertAssetMappingProduct_Details",
                        new { Product_Id = productId, Ticket_Id = ticketId },
                        commandType: CommandType.StoredProcedure);
                }
            }

            return new RaiseAssetResponseDto
            {
                RequestId = ticketId,
                Message = ticketId > 0 ? "Asset request raised successfully." : "Failed to raise asset request."
            };
        }
        public async Task<UpdateAssetStatusResponseDto> UpdateAssetStatusAsync(UpdateAssetStatusRequestDto request)
        {
            using var conn = _db.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_UpdateAssestsUsersDetail",
                new { Id = request.ReqId, status = request.Status },
                commandType: CommandType.StoredProcedure);

            return new UpdateAssetStatusResponseDto
            {
                Message = result != null && result.RowsUpdated > 0
        ? "Status updated successfully."
        : "No record found."
            };
        }

        public async Task<bool> AddAssetAsync(AddAssetRequestDto request)
        {
            using var conn = _db.CreateConnection();

            if (!int.TryParse(request.P_Id, out int projectId))
                throw new ArgumentException(
                    $"Invalid P_Id: {request.P_Id}");

            bool isActive;

            if (request.IsActive == "1")
                isActive = true;
            else if (request.IsActive == "0")
                isActive = false;
            else if (!bool.TryParse(request.IsActive, out isActive))
                throw new ArgumentException(
                    $"Invalid IsActive: {request.IsActive}");

            await conn.ExecuteAsync(
                "dbo.Sp_InsertAssestsDetails",
                new
                {
                    P_Id = projectId,
                    Product_Name = request.Product_Name,
                    Service_Number = request.Service_Number,
                    Storage = request.Storage,
                    IsActive = isActive
                },
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }
}
