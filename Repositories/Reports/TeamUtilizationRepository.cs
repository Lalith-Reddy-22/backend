using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Reports
{
    public class TeamUtilizationRepository : ITeamUtilization
    {
        private readonly DbContext.DbContext _dbContext;

        public TeamUtilizationRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TeamUtilizationResponseDto> GetTeamUtilizationSummaryAsync(TeamUtilizationRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@start_date", request.StartDate.Date, DbType.Date);
            parameters.Add("@end_date", request.EndDate.Date, DbType.Date);

            // SP returns a single result set with SummaryType = 'Billable' or 'Non-Billable'
            var rows = await connection.QueryAsync<TeamUtilizationRowDto>(
                "sp_utilization_summary_report",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var allRows = rows.ToList();

            return new TeamUtilizationResponseDto
            {
                Billable    = allRows.Where(r => r.SummaryType == "Billable").ToList(),
                NonBillable = allRows.Where(r => r.SummaryType == "Non-Billable").ToList()
            };
        }
    }
}
