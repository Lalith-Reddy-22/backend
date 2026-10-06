using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Reports
{
    public class DailyReports : IDailyReports
    {
        private readonly DbContext.DbContext _dbContext;

        public DailyReports(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<DailyReportDto>> GetDailyTaskDetailsAsync(
    DailyReportRequestDto request)
        {
            try
            {
                using IDbConnection connection = _dbContext.CreateConnection();

                var parameters = new DynamicParameters();

                parameters.Add("@TodayDate", request.TodayDate, DbType.DateTime);
                parameters.Add("@TaskId", request.TaskId, DbType.Int32);
                parameters.Add("@ResourceName", request.ResourceName, DbType.String);
                parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
                parameters.Add("@PageSize", request.PageSize, DbType.Int32);

                using var multi = await connection.QueryMultipleAsync(
                    "PMTool_v2.New_v2_sp_get_all_Dailytask_details",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // First result = current page of task rows
                var result = (await multi.ReadAsync<DailyReportDto>())
                    .ToList();

                // Second result = total number of matching task rows
                var totalRecords = await multi.ReadSingleAsync<int>();

                return new PagedResult<DailyReportDto>
                {
                    TotalRecords = totalRecords,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== DAILY REPORT ERROR ==========");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("========================================");

                throw;
            }
        }
    }
}