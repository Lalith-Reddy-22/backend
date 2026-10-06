using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.ResourceAllocation;
using Medtrix_PmTool_V2_Api.Interfaces.ResourceAllocation;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.ResourceAllocation
{
    public class ResourceAllocationRepository : IResourceAllocation
    {
        private readonly DbContext.DbContext _dbContext;

        public ResourceAllocationRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET /api/resource-allocation?startDate=2026-01-02&endDate=2026-01-20&teamId=1
        // SP: [PMTool_v2].[New_v2_sp_get_log_sheet_data_simple]
        // Table 1: Employee list with totals
        // Table 2: Per-date detail rows 
        public async Task<ResourceAllocationResponseDto> GetResourceAllocationAsync(ResourceAllocationRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@start_date", request.StartDate, DbType.Date);
            parameters.Add("@end_date",   request.EndDate,   DbType.Date);
            parameters.Add("@team_id",    request.TeamId,    DbType.Int32);

            using var multi = await connection.QueryMultipleAsync(
                "PMTool_v2.New_v2_sp_get_log_sheet_data_simple",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Table 1 — employee list with totals
            var empList    = (await multi.ReadAsync<RawEmpTotalDto>()).ToList();

            // Table 2 — per-date daily detail 
            var dailyData  = (await multi.ReadAsync<RawDailyDetailDto>()).ToList();

            // Group daily rows by EmpId and build response
            var employees = empList.Select(emp =>
            {
                var empDays = dailyData
                    .Where(d => d.EmpId == emp.EmpId)
                    .Select(d => new DateCellDto
                    {
                        date      = d.Date,
                        planned   = d.Planned_Hours,
                        logged    = d.Logged_Hours,
                        available = d.Available_Hours -d.Planned_Hours
                    })
                    .ToList();

                return new EmployeeRowDto
                {
                    EmpId         = emp.EmpId,
                    Employee_Name = emp.Employee_Name,
                    total_planned = emp.Total_Planned_Hours,
                    total_logged  = emp.Total_Logged_Hours,
                    dates         = empDays
                };
            }).ToList();

            return new ResourceAllocationResponseDto { employees = employees };
        }

        // GET /api/resource-allocation/task-detail?empId=3&dateVal=2024-09-27
        // SP: [dbo].[sp_get_emp_task_logged_data]
        // Returns 2 result sets: task rows + employee header
        public async Task<EmpTaskDetailResponseDto> GetEmpTaskDetailAsync(EmpTaskDetailRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",   request.EmpId,   DbType.Int32);
            parameters.Add("@date_val", request.DateVal, DbType.Date);

            using var multi = await connection.QueryMultipleAsync(
                "sp_get_emp_task_logged_data",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var tasks  = (await multi.ReadAsync<EmpTaskDetailRowDto>()).ToList();
            var header = !multi.IsConsumed
                ? (await multi.ReadAsync<EmpTaskHeaderDto>()).FirstOrDefault()
                : null;

            return new EmpTaskDetailResponseDto
            {
                Header = header,
                Tasks  = tasks
            };
        }
    }
}
