using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Reports
{
    public class WeekUtilizationRepository : IWeekUtilization
    {
        private readonly DbContext.DbContext _dbContext;

        public WeekUtilizationRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WeekUtilizationResponseDto> GetUtilizationSummaryAsync(WeekUtilizationRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@start_date", request.StartDate.Date, DbType.Date);
            parameters.Add("@end_date", request.EndDate.Date, DbType.Date);

            // SP returns two result sets: Billable first, Non-Billable second
            using var multi = await connection.QueryMultipleAsync(
                "sp_utilization_summary_by_designation",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var billable = (await multi.ReadAsync<UtilizationRowDto>()).ToList();
            var nonBillable = (await multi.ReadAsync<UtilizationRowDto>()).ToList();

            return new WeekUtilizationResponseDto
            {
                Billable = billable,
                NonBillable = nonBillable
            };
        }
    }
}
