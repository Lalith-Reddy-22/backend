using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Reports
{
    public class ClientRevenueRepository : IClientRevenue
    {
        private readonly DbContext.DbContext _dbContext;

        public ClientRevenueRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ClientRevenueRowDto>> GetClientWisePlannedRevenueAsync(ClientRevenueRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@start_date", request.StartDate.Date, DbType.Date);
            parameters.Add("@end_date", request.EndDate.Date, DbType.Date);

            return await connection.QueryAsync<ClientRevenueRowDto>(
                "sp_clientwise_planned_revenue_report",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
