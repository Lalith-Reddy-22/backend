using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Leave;
using Medtrix_PmTool_V2_Api.DTOs.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.Leave;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Leave
{
    public class LeaveRepository : ILeave
    {
        private readonly DbContext.DbContext _dbContext;

        public LeaveRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /api/leave/teams
        public async Task<IEnumerable<TeamDropdownDto>> GetTeamsDropdownAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<TeamDropdownDto>(
                "SP_BindTeamsDrpdwn",
                commandType: CommandType.StoredProcedure
            );
        }

        // GET: /api/leave/designations?teamId=1
        public async Task<IEnumerable<DesignationDropdownDto>> GetDesignationsByTeamAsync(int teamId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId, DbType.Int32);
            return await connection.QueryAsync<DesignationDropdownDto>(
                "Sp_Get_designation_Drp",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // GET: /api/leave?teamName=Tech&pageNumber=1&pageSize=10
        public async Task<PagedResult<LeaveListDto>> GetAllLeavesAsync(LeaveListRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TeamName",   request.TeamName,   DbType.String);
            parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
            parameters.Add("@PageSize",   request.PageSize,   DbType.Int32);

            var rows = await connection.QueryAsync<LeaveListDto>(
                "PMTool_v2.New_v2_Sp_GetDesignLeavesList",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var list = rows.ToList();
            int totalRecords = list.FirstOrDefault()?.TotalRecords ?? 0;

            return new PagedResult<LeaveListDto>
            {
                TotalRecords = totalRecords,
                PageNumber   = request.PageNumber,
                PageSize     = request.PageSize,
                Data         = list
            };
        }

        // GET: /api/leave/{id}
        public async Task<LeaveDetailDto?> GetLeaveByIdAsync(int leaveId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveId", leaveId, DbType.Int32);
            return await connection.QueryFirstOrDefaultAsync<LeaveDetailDto>(
                "SP_GetDesignLeaveDataById",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // POST: /api/leave  → Insert (LeaveId = 0)
        public async Task<string> CreateLeaveAsync(CreateLeaveRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveId",               0,                             DbType.Int32);
            parameters.Add("@TeamId",                request.TeamId,                DbType.Int32);
            parameters.Add("@Designation",           request.Designation,           DbType.String);
            parameters.Add("@DesigBillableValue",    request.BillableLeaveHours,    DbType.Int32);
            parameters.Add("@DesigNonBillableValue", request.NonBillableLeaveHours, DbType.Int32);
            parameters.Add("@startDate",             request.StartDate,             DbType.String);
            parameters.Add("@endDate",               request.EndDate,               DbType.String);
            parameters.Add("@msg", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_CreateDesigLeaveData", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<string>("@msg") ?? string.Empty;
        }

        // PUT: /api/leave/{id}  → Update (LeaveId from route)
        public async Task<string> UpdateLeaveAsync(int leaveId, UpdateLeaveRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeaveId",               leaveId,                       DbType.Int32);
            parameters.Add("@TeamId",                request.TeamId,                DbType.Int32);
            parameters.Add("@Designation",           request.Designation,           DbType.String);
            parameters.Add("@DesigBillableValue",    request.BillableLeaveHours,    DbType.Int32);
            parameters.Add("@DesigNonBillableValue", request.NonBillableLeaveHours, DbType.Int32);
            parameters.Add("@startDate",             request.StartDate,             DbType.String);
            parameters.Add("@endDate",               request.EndDate,               DbType.String);
            parameters.Add("@msg", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_CreateDesigLeaveData", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<string>("@msg") ?? string.Empty;
        }
    }
}
