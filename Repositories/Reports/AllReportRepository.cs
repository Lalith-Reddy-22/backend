using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Reports
{
    public class AllReportRepository : IAllReport
    {
        private readonly DbContext.DbContext _dbContext;

        public AllReportRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Projects dropdown 
  
        public async Task<IEnumerable<ProjectDropdownDto>> GetProjectsDropdownAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<ProjectDropdownDto>(
                "SP_GetProjectsDropdown",
                commandType: CommandType.StoredProcedure
            );
        }

        //  Task list
        // SP: [PMTool_v2].[New_v2_Sp_GetTaskListReport]
        // Params: @emp_id, @emp_type, @start_date, @end_date, @projId,
        //         @search_text, @page_number, @page_size, @total_count OUTPUT
        public async Task<ReportPagedResult<TaskReportRowDto>> GetTaskListAsync(TaskListRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",      request.EmpId,      DbType.Int32);
            parameters.Add("@emp_type",    request.EmpType,    DbType.Int32);
            parameters.Add("@start_date",  request.StartDate,  DbType.Date);
            parameters.Add("@end_date",    request.EndDate,    DbType.Date);
            parameters.Add("@projId",      request.ProjectId,  DbType.Int32);
            parameters.Add("@search_text", request.SearchText, DbType.String);
            parameters.Add("@page_number", request.PageNumber, DbType.Int32);
            parameters.Add("@page_size",   request.PageSize,   DbType.Int32);
            parameters.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rows = await connection.QueryAsync<TaskReportRowDto>(
                "PMTool_v2.New_v2_Sp_GetTaskListReport",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int totalCount = parameters.Get<int>("@total_count");

            return new ReportPagedResult<TaskReportRowDto>
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize,
                Data       = rows.ToList()
            };
        }

        //: Employee list 
        // SP: [PMTool_v2].[New_v2_Sp_GetEmployeeWiseReport]
        // Params: @start_date, @end_date, @search_text, @page_number, @page_size, @total_count OUTPUT
        public async Task<ReportPagedResult<ReportEmployeeDto>> GetEmployeeListAsync(EmployeeListRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@start_date",  request.StartDate,  DbType.Date);
            parameters.Add("@end_date",    request.EndDate,    DbType.Date);
            parameters.Add("@search_text", request.SearchText, DbType.String);
            parameters.Add("@page_number", request.PageNumber, DbType.Int32);
            parameters.Add("@page_size",   request.PageSize,   DbType.Int32);
            parameters.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rows = await connection.QueryAsync<ReportEmployeeDto>(
                "PMTool_v2.New_v2_Sp_GetEmployeeWiseReport",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int totalCount = parameters.Get<int>("@total_count");

            return new ReportPagedResult<ReportEmployeeDto>
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize,
                Data       = rows.ToList()
            };
        }

        // Project list
        // SP: [PMTool_v2].[New_v2_Sp_GetProjectWiseReport]
        // Params: @emp_id, @emp_type, @search_text, @page_number, @page_size, @total_count OUTPUT
        public async Task<ReportPagedResult<ReportProjectDto>> GetProjectListAsync(
    ProjectListRequestDto request)
        {
            var totalStart = DateTime.Now;

            Console.WriteLine("===== PROJECT LIST START =====");
            Console.WriteLine($"Start: {DateTime.Now:HH:mm:ss.fff}");

            using SqlConnection connection =
                (SqlConnection)_dbContext.CreateConnection();
            Console.WriteLine($"Connection created: {DateTime.Now:HH:mm:ss.fff}");

            var parameters = new DynamicParameters();

            parameters.Add("@emp_id", request.EmpId, DbType.Int32);
            parameters.Add("@emp_type", request.EmpType, DbType.Int32);
            parameters.Add("@search_text", request.SearchText, DbType.String);
            parameters.Add("@page_number", request.PageNumber, DbType.Int32);
            parameters.Add("@page_size", request.PageSize, DbType.Int32);
            parameters.Add(
                "@total_count",
                dbType: DbType.Int32,
                direction: ParameterDirection.Output
            );

            var sqlStart = DateTime.Now;

            var rows = await connection.QueryAsync<ReportProjectDto>(
                "PMTool_v2.New_v2_Sp_GetProjectWiseReport",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120
            );

            var sqlEnd = DateTime.Now;

            Console.WriteLine(
                $"PROJECT SQL TIME: {(sqlEnd - sqlStart).TotalSeconds:F2} seconds"
            );

            var rowsList = rows.ToList();

            Console.WriteLine($"Rows received: {rowsList.Count}");

            int totalCount = parameters.Get<int>("@total_count");

            Console.WriteLine($"Total count: {totalCount}");

            var result = new ReportPagedResult<ReportProjectDto>
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Data = rowsList
            };

            var totalEnd = DateTime.Now;

            Console.WriteLine("===== PROJECT LIST END =====");
            Console.WriteLine(
                $"Total API repository time: {(totalEnd - totalStart).TotalSeconds:F2} seconds"
            );

            return result;
        }

        //  Employee task details 
        // SP: spGetEmpTaskListByTaskID
        public async Task<IEnumerable<EmpTaskModalRowDto>> GetEmpTaskDetailsAsync(EmpTaskDetailModalRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@empid",     request.EmployeeId, DbType.Int32);
            parameters.Add("@startDate", request.StartDate,  DbType.String);
            parameters.Add("@endDate",   request.EndDate,    DbType.String);

            return await connection.QueryAsync<EmpTaskModalRowDto>(
                "spGetEmpTaskListByTaskID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // API 6: Project task details 
        // SP: SP_GetAllTaskListForReport
        // Params: @ProjectId, @start_date, @end_date
        public async Task<IEnumerable<ProjectTaskDetailRowDto>> GetProjectTaskDetailsAsync(ProjectTaskDetailRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ProjectId",   request.ProjectId, DbType.Int32);
            parameters.Add("@start_date",  request.StartDate, DbType.Date);
            parameters.Add("@end_date",    request.EndDate,   DbType.Date);

            return await connection.QueryAsync<ProjectTaskDetailRowDto>(
                "SP_GetAllTaskListForReport",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
