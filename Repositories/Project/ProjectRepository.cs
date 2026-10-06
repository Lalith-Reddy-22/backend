using Dapper;
using Medtrix_PmTool_V2_Api.DbContext;
using Medtrix_PmTool_V2_Api.DTOs.Project;
using Medtrix_PmTool_V2_Api.Interfaces.Project;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Project
{
    public class ProjectRepository : IProject
    {
        private readonly DbContext.DbContext _dbContext;

        public ProjectRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectCostInfoResponseDto> GetProjectCostInfoAsync(int projectId, int buyingCenterId)
        {
            using var conn = _dbContext.CreateConnection();

            var costRow = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "GetProjectDetailsById",
                new { projectid = projectId },
                commandType: CommandType.StoredProcedure);



            double totalValue       = costRow?.Total_Value       != null ? (double)costRow.Total_Value       : 0;
            double predictedValue   = costRow?.Predicted_Value   != null ? (double)costRow.Predicted_Value   : 0;
            double outsourcingValue = costRow?.Outsourcing_Value  != null ? (double)costRow.Outsourcing_Value  : 0;

            double utilizationCost = 0;
            if (buyingCenterId > 0)
            {
                var utilParams = new DynamicParameters();
                utilParams.Add("@proj_id", projectId,      DbType.Int32);
                utilParams.Add("@b_id",    buyingCenterId, DbType.Int32);

                var utilRows = await conn.QueryAsync<dynamic>(
                    "sp_get_utilization_data",
                    utilParams,
                    commandType: CommandType.StoredProcedure);

                utilizationCost = utilRows
                    .Where(r => r.Utilization != null)
                    .Sum(r => (double)r.Utilization);
            }

            return new ProjectCostInfoResponseDto
            {
                TotalValue       = totalValue,
                PredictedValue   = predictedValue,
                OutsourcingValue = outsourcingValue,
                UtilizationCost  = utilizationCost
            };
        }

        public async Task<GetAllProjectsResponseDto> GetAllProjectsAsync(GetAllProjectsRequestDto request)
        {
            using var conn = _dbContext.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@search_text", string.IsNullOrWhiteSpace(request.SearchText) ? null : request.SearchText, DbType.String);
            p.Add("@page_number", request.PageNumber, DbType.Int32);
            p.Add("@page_size",   request.PageSize,   DbType.Int32);
            p.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rows = (await conn.QueryAsync<ProjectListItemDto>(
                "PMTool_v2.sp_get_all_projects",
                p,
                commandType: CommandType.StoredProcedure)).ToList();

            foreach (var row in rows)
            {
                Console.WriteLine(
                    $"ProjectId = {row.Project_Id}, B_Id = {row.B_Id}"
                );
            }

            int totalCount = p.Get<int>("@total_count");
            int totalPages = request.PageSize > 0 ? (int)Math.Ceiling((double)totalCount / request.PageSize) : 0;

            return new GetAllProjectsResponseDto
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Projects   = rows
               
            };
        }
        public async Task<DeleteProjectResponseDto> DeleteProjectAsync(int projectId, DeleteProjectRequestDto request)
        {
            using var conn = _dbContext.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@ProjectID",    projectId,             DbType.Int32);
            p.Add("@DeletedBy",    request.DeletedBy,     DbType.Int32);
            p.Add("@DeleteReason", request.DeleteReason,  DbType.String);
            p.Add("@Message",      dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

            var row = await conn.QueryFirstOrDefaultAsync<DeleteProjectResponseDto>(
                "PMTool_v2.sp_delete_project",
                p,
                commandType: CommandType.StoredProcedure);

            string message = p.Get<string>("@Message");

            if (row != null)
                return row;

            return new DeleteProjectResponseDto
            {
                ProjectId = projectId,
                Message   = message ?? "Project not found."
            };
        }

        public async Task<IEnumerable<ProjectTaskDto>> GetProjectTasksAsync(
    int projectId,
    int empId,
    int userType)
        {
            using var conn = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@emp_id", empId, DbType.Int32);
            parameters.Add("@usr_typ", userType, DbType.Int32);
            parameters.Add("@project_Id", projectId, DbType.Int32);

            var tasks = await conn.QueryAsync<ProjectTaskDto>(
                "sp_get_all_task_details_ProjectId",
                parameters,
                commandType: CommandType.StoredProcedure);

            return tasks.ToList();
        }
    }
}
