using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Finance
{
    public class FinanceProjectList2Repository : IFinanceProjectList2
    {
        private readonly DbContext.DbContext _db;

        public FinanceProjectList2Repository(DbContext.DbContext db)
        {
            _db = db;
        }

        public async Task<ProjectListResponse2Dto> GetProjectListAsync(ProjectListRequestDto request)
        {
            using var conn = _db.CreateConnection();

            var bp = new DynamicParameters();
            bp.Add("@Emp_Id",            request.EmpId,                                                             DbType.Int32);
            bp.Add("@search_text",       string.IsNullOrWhiteSpace(request.SearchText) ? null : request.SearchText, DbType.String);
            bp.Add("@client_id",         null, DbType.Int32);
            bp.Add("@financial_year_id", null, DbType.Int32);
            bp.Add("@project_status",    null, DbType.Int32);
            bp.Add("@page_number",       request.PageNumber, DbType.Int32);
            bp.Add("@page_size",         request.PageSize,   DbType.Int32);
            bp.Add("@total_count",       dbType: DbType.Int32, direction: ParameterDirection.Output);

            var baseProjects = (await conn.QueryAsync<BaseProjectRawDto>(
                "PMTool_v2.sp_get_base_projects",
                bp,
                commandType: CommandType.StoredProcedure
            )).ToList();

            if (!baseProjects.Any())
                return new ProjectListResponse2Dto
                {
                    EmpId      = request.EmpId,
                    PageNumber = request.PageNumber,
                    PageSize   = request.PageSize
                };

            var projectIds = string.Join(",", baseProjects.Select(p => p.PROJECT_ID));
            int totalCount = baseProjects[0].TotalCount;
            int totalPages = baseProjects[0].TotalPages;

            var ep = new DynamicParameters();
            ep.Add("@ProjectIds",  projectIds,   DbType.String);
            ep.Add("@page_number", 1,             DbType.Int32);
            ep.Add("@page_size",   int.MaxValue,  DbType.Int32);
            ep.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var empValues = (await conn.QueryAsync<EmployeeValueRawDto>(
                "PMTool_v2.sp_get_employee_values",
                ep,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var ap = new DynamicParameters();
            ap.Add("@ProjectIds",  projectIds,   DbType.String);
            ap.Add("@page_number", 1,             DbType.Int32);
            ap.Add("@page_size",   int.MaxValue,  DbType.Int32);
            ap.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var actualLogs = (await conn.QueryAsync<ActualLogRawDto>(
                "PMTool_v2.sp_get_actual_logs",
                ap,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var tp = new DynamicParameters();
            tp.Add("@ProjectIds",  projectIds,   DbType.String);
            tp.Add("@page_number", 1,             DbType.Int32);
            tp.Add("@page_size",   int.MaxValue,  DbType.Int32);
            tp.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var taskSummary = (await conn.QueryAsync<TaskSummaryRawDto>(
                "PMTool_v2.sp_get_project_task_summary",
                tp,
                commandType: CommandType.StoredProcedure
            )).ToDictionary(t => t.ProjectId);

            var predictedCost = empValues
                .GroupBy(e => e.project_id)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Working_hours * e.Emp_Value));

            var actualCost = actualLogs
                .GroupBy(a => a.project_id)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.Log_hours * a.Emp_Value));

            var projects = baseProjects.Select(p =>
            {
                var predCost = predictedCost.GetValueOrDefault(p.PROJECT_ID, 0);
                var actCost  = actualCost.GetValueOrDefault(p.PROJECT_ID, 0);
                var task     = taskSummary.GetValueOrDefault(p.PROJECT_ID);

                double predGP  = p.Total_Value - predCost - p.Outsourcing_Value;
                double actGP   = p.Total_Value - actCost  - p.Outsourcing_Value;

                double predPct = p.Total_Value != 0 ? Math.Round(predGP / p.Total_Value * 100, 2) : 0;
                double actPct  = p.Total_Value != 0 ? Math.Round(actGP  / p.Total_Value * 100, 2) : 0;

                return new ProjectListItemDto2
                {
                    PROJECT_ID           = p.PROJECT_ID,
                    projectCode          = p.projectCode,
                    project_Title        = p.project_Title,
                    Region               = p.Region,
                    project_desc         = p.project_desc,
                    Pstart_date          = p.start_date?.ToString("MM/dd/yy"),
                    end_date             = p.end_date?.ToString("MM/dd/yy"),
                    Created_Date         = p.Created_Date?.ToString("MM/dd/yy"),
                    Project_Status       = p.Project_Status,
                    Client_Title         = p.Client_Title,
                    B_Name               = p.B_Name,
                    Total_Value          = p.Total_Value,
                    Outsourcing_Value    = p.Outsourcing_Value,
                    Vendor_Value         = p.Vendor_Value,
                    Predicted_GP         = Math.Round(predGP, 2),
                    Predicted_percentage = predPct,
                    Actual_GP            = Math.Round(actGP,  2),
                    Actual_percentage    = actPct,
                    TotalTasks           = task?.TotalTasks     ?? 0,
                    CompletedTasks       = task?.CompletedTasks ?? 0,
                    OpenTasks            = task?.OpenTasks      ?? 0
                };
            });

            return new ProjectListResponse2Dto
            {
                EmpId      = request.EmpId,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize,
                Projects   = projects
            };
        }
    }
}
