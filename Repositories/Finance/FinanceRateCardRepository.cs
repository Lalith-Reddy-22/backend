using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Finance
{
    public class FinanceRateCardRepository : IFinanceRateCard
    {
        private readonly DbContext.DbContext _db;

        public FinanceRateCardRepository(DbContext.DbContext db)
        {
            _db = db;
        }

        // GET /api/v1/finance-rate-card?teamId=1
        // SP uses dynamic PIVOT — columns vary per team based on BName values
        // Returns list of dictionaries to handle dynamic column names
        public async Task<object> GetRateCardByTeamAsync(int teamId)
        {
            using var conn = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Team_ID", teamId, DbType.Int32);

            var rows = (await conn.QueryAsync(
                "PMTool_v2.SP_RATECARD_VALUE_BNAME_BY_TEAM",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            // Convert each DapperRow to a plain Dictionary<string, object?>
            // so JSON serialization includes all dynamic pivot columns
            var result = rows
                .Select(r => (IDictionary<string, object?>)(IDictionary<string, object>)r)
                .Select(d => d.ToDictionary(k => k.Key, k => k.Value))
                .ToList();

            return new
            {
                TeamId     = teamId,
                TeamType   = GetTeamType(teamId),
                TotalCount = result.Count,
                Data       = result
            };
        }

        private static string GetTeamType(int teamId) => teamId switch
        {
            1       => "Tech",
            2       => "Creative",
            3       => "Content",
            4 or 5 or 6 => "PM",
            _       => "Unknown"
        };

        // GET /api/v1/finance-rate-card/buying-centers
        public async Task<IEnumerable<BuyingCenterDetailDto>> GetBuyingCenterDetailsAsync()
        {
            using var conn = _db.CreateConnection();

            return await conn.QueryAsync<BuyingCenterDetailDto>(
                "Sp_GetBuyingCenterDetails",
                commandType: CommandType.StoredProcedure
            );
        }

        // POST /api/v1/finance-rate-card/buying-centers/clone
        public async Task<CloneBuyingCenterResponseDto> CloneBuyingCenterAsync(CloneBuyingCenterRequestDto request)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@OrgBId",        request.OrgBId,        DbType.Int32);
            p.Add("@BuyCenterName", request.BuyCenterName, DbType.String);
            p.Add("@CreatedBy",     request.CreatedBy,     DbType.Int32);

            await conn.ExecuteAsync(
                "SP_CreateRateCardClone_Bid",
                p,
                commandType: CommandType.StoredProcedure
            );

            return new CloneBuyingCenterResponseDto
            {
                Success = true,
                Message = $"Buying center '{request.BuyCenterName}' cloned successfully from BId {request.OrgBId}."
            };
        }
    }
}
