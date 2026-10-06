using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Finance
{
    public class FinanceRepository : IFinance
    {
        private readonly DbContext.DbContext _db;

        public FinanceRepository(DbContext.DbContext db)
        {
            _db = db;
        }

        public async Task<GetClientGpResponseDto> GetClientGpAsync(int ? finYrId, string? searchText, int pageNumber, int pageSize)
        {
            if(finYrId == null)
            {
                finYrId = 0;
            }
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@FinYrId",      finYrId,                                                    DbType.Int32);
            p.Add("@search_text",  string.IsNullOrWhiteSpace(searchText) ? null : searchText,  DbType.String);
            p.Add("@page_number",  pageNumber,                                                  DbType.Int32);
            p.Add("@page_size",    pageSize,                                                    DbType.Int32);
            p.Add("@total_count",  dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rows = (await conn.QueryAsync<ClientGpItemDto>(
                "PMTool_v2.sp_get_client_gp_details",
                p,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalCount = p.Get<int>("@total_count");
            int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

            return new GetClientGpResponseDto
            {
                Clients    = rows,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize   = pageSize
            };
        }
        public async Task<IEnumerable<FinancialYearDto>> GetAllFinancialYearsAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<FinancialYearDto>(
                "PMTool_v2.sp_get_all_financial_years_simple",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<GetClientProjectsResponseDto> GetProjectsByClientAsync(int clientId, int? finYrId, int? projectStatus, string? searchText, int pageNumber, int pageSize)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@clientID",       clientId,                                                  DbType.Int32);
            p.Add("@search_text",    string.IsNullOrWhiteSpace(searchText) ? null : searchText, DbType.String);
            p.Add("@project_status", projectStatus,                                             DbType.Int32);
            p.Add("@page_number",    pageNumber,                                                DbType.Int32);
            p.Add("@page_size",      pageSize,                                                  DbType.Int32);
            p.Add("@total_count",    dbType: DbType.Int32, direction: ParameterDirection.Output);

            string spName;

            if (finYrId.HasValue && finYrId.Value > 0)
            {
                p.Add("@FinYrId", finYrId.Value, DbType.Int32);
                spName = "PMTool_v2.sp_get_projects_by_client_financial_year_updated";
            }
            else
            {
                spName = "PMTool_v2.sp_get_projects_by_client_updated";
            }

            var rows = (await conn.QueryAsync<ClientProjectItemDto>(
                spName,
                p,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalCount = p.Get<int>("@total_count");
            int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

            return new GetClientProjectsResponseDto
            {
                Projects   = rows,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize   = pageSize
            };
        }

        // GET /api/v1/finance/client-projects?clientId=1&finYrId=2
        // Uses spGetProjectDetailsByClientID_FinYrId (with finYr) or spGetProjectDetailsByClientID (without)
        public async Task<ClientProjectsBySpResponseDto> GetClientProjectsBySpAsync(int clientId, int? finYrId)
        {
            using var conn = _db.CreateConnection();

            IEnumerable<ClientProjectBySpDto> projects;

            if (finYrId.HasValue && finYrId.Value > 0)
            {
                var p = new DynamicParameters();
                p.Add("@clientID", clientId,       DbType.Int32);
                p.Add("@FinYrId",  finYrId.Value,  DbType.Int32);

                projects = await conn.QueryAsync<ClientProjectBySpDto>(
                    "spGetProjectDetailsByClientID_FinYrId",
                    p,
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var p = new DynamicParameters();
                p.Add("@clientID", clientId, DbType.Int32);

                projects = await conn.QueryAsync<ClientProjectBySpDto>(
                    "spGetProjectDetailsByClientID",
                    p,
                    commandType: CommandType.StoredProcedure);
            }

            return new ClientProjectsBySpResponseDto
            {
                ClientId = clientId,
                FinYrId  = finYrId,
                Projects = projects
            };
        }

        // GET /api/v1/finance/project-task-report?projId=101&pageNumber=1&pageSize=10
        public async Task<ProjectTaskReportResponseDto> GetProjectTaskReportAsync(int projId, string? searchText, int pageNumber, int pageSize)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@projId",      projId,                                                     DbType.Int32);
            p.Add("@search_text", string.IsNullOrWhiteSpace(searchText) ? null : searchText,  DbType.String);
            p.Add("@page_number", pageNumber,                                                  DbType.Int32);
            p.Add("@page_size",   pageSize,                                                    DbType.Int32);
            p.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var tasks = (await conn.QueryAsync<ProjectTaskReportItemDto>(
                "PMTool_v2.sp_get_project_task_report",
                p,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalCount = p.Get<int>("@total_count");
            int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

            return new ProjectTaskReportResponseDto
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize   = pageSize,
                TotalPages = totalPages,
                Tasks      = tasks
            };
        }

        public async Task<string> SaveClientAsync(CreateClientRequestDto request)
        {
            using var connection = _db.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@client", request.ClientName);
            parameters.Add("@client_Id", request.ClientId);
            parameters.Add("@Descp", request.Description);
            parameters.Add("@Action", request.Action);

            parameters.Add("@msg",
                dbType: DbType.String,
                size: 100,
                direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "SP_ClientDetails",
                parameters,
                commandType: CommandType.StoredProcedure);

            var status = parameters.Get<string>("@msg");

            if (status == "Inserted" || status == "Updated")
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO Tbl_UserActivities
              (Description, Activity, Created_On, Create_By)
              VALUES
              (@Description, @Activity, GETDATE(), @CreateBy)",
                    new
                    {
                        Description = request.Action == "Insert"
                            ? "Inserted client"
                            : "Updated client",

                        Activity = request.Action,
                        CreateBy = request.CreatedBy
                    });
            }

            return status;
        }

        public async Task<ClientDetailDto?> GetClientByIdAsync(int clientId)
        {
            using var conn = _db.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@clientId", clientId);

            return await conn.QueryFirstOrDefaultAsync<ClientDetailDto>(
                "SP_GetClientDetailsById",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
