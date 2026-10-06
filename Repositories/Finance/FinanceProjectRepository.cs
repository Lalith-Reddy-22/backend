using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Finance
{
    public class FinanceProjectRepository : IFinanceProject
    {
        private readonly DbContext.DbContext _db;

        public FinanceProjectRepository(DbContext.DbContext db)
        {
            _db = db;
        }

        // ─── SP_BindUsersClientData: single call, all 5 lookups ───────────────
        // Tables: [0]=skip, [1]=Clients, [2]=skip, [3]=PMs, [4]=BuyingCenters,
        //         [5]=FinancialYears, [6]=ProjectStatuses
        public async Task<ProjectLookupsResponseDto> GetProjectLookupsAsync()
        {
            using var conn  = _db.CreateConnection();
            using var multi = await conn.QueryMultipleAsync(
                "SP_BindUsersClientData",
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadAsync<dynamic>();                                           // [0] all employees — skip
            var clients        = (await multi.ReadAsync<LookupClientDto>()).ToList();   // [1] Client_ID, Client_Title
            await multi.ReadAsync<dynamic>();                                           // [2] non-admin employees — skip
            var managers       = (await multi.ReadAsync<LookupEmployeeDto>()).ToList(); // [3] EmpId, Employee_Name
            var buyingCenters  = (await multi.ReadAsync<LookupBuyingCenterDto>()).ToList(); // [4] BId, BName
            var financialYears = (await multi.ReadAsync<LookupFinancialYearDto>()).ToList(); // [5] Id, Financial_Year

            // Read as dynamic — Tbl_ProjectStatus column name for status text is unknown
            var statusRaw = (await multi.ReadAsync<dynamic>()).ToList(); // [6]
            var statuses = statusRaw.Select(r =>
            {
                var dict = (IDictionary<string, object>)r;
                // Try all likely column name variants
                string name = "";
                foreach (var key in new[] { "Status_Name", "StatusName", "Status", "Project_Status", "ProjectStatus", "Name", "project_status_name" })
                {
                    if (dict.TryGetValue(key, out var val) && val != null)
                    { name = val.ToString()!; break; }
                }
                return new LookupProjectStatusDto
                {
                    Id          = dict.TryGetValue("Id", out var id) ? Convert.ToInt32(id) : 0,
                    Status_Name = name
                };
            }).ToList();

            return new ProjectLookupsResponseDto
            {
                Clients         = clients,
                ProjectManagers = managers,
                BuyingCenters   = buyingCenters,
                FinancialYears  = financialYears,
                ProjectStatuses = statuses
            };
        }

        // ─── GetProjectDetailsById ────────────────────────────────────────────
        public async Task<FinanceProjectDetailDto?> GetProjectByIdAsync(int projectId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<FinanceProjectDetailDto>(
                "GetProjectDetailsById",
                new { projectid = projectId },
                commandType: CommandType.StoredProcedure
            );
        }

        // ─── PMTool_v2.sp_save_project — Insert or Update ────────────────────
        public async Task<SaveProjectResponseDto> SaveProjectAsync(string action, int? projectId, SaveProjectRequestDto r)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@ProjectId",          projectId,           DbType.Int32);
            p.Add("@title",              r.ProjectTitle,      DbType.String);
            p.Add("@ClientId",           r.ClientId,          DbType.Int32);
            p.Add("@ProjectCode",        r.ProjectCode,       DbType.String);
            p.Add("@startDate",          r.StartDate,         DbType.String);
            p.Add("@endDate",            r.EndDate,           DbType.String);
            p.Add("@Action",             action,              DbType.String);
            p.Add("@ProjectManager",     r.ProjectManagerIds, DbType.String);
            p.Add("@BuyingCenterId",     r.BuyingCenterId,    DbType.Int32);
            p.Add("@Predicted_Value",    r.PredictedValue,    DbType.Double);
            p.Add("@Outsourcing_Value",  r.OutsourcingValue,  DbType.Double);
            p.Add("@Total_Value",        r.TotalValue,        DbType.Double);
            p.Add("@Vendor_Value",       r.VendorValue,       DbType.Double);
            p.Add("@Created_by",         r.CreatedBy,         DbType.Int32);
            p.Add("@Financial_Year",     r.FinancialYearId,   DbType.Int32);
            p.Add("@Descp",              r.Description,       DbType.String);
            p.Add("@Region",             r.Region,            DbType.String);
            p.Add("@Status",             r.ProjectStatus,     DbType.Int32);
            p.Add("@Type",               r.BillingType,       DbType.Int32);
            p.Add("@SubClient",          r.SubClientName,     DbType.String);
            p.Add("@msg",          dbType: DbType.String, size: 50,  direction: ParameterDirection.Output);
            p.Add("@ReturnProjectId", dbType: DbType.Int32,          direction: ParameterDirection.Output);

            // SP returns a result row — read it for the response
            var row = await conn.QueryFirstOrDefaultAsync<SaveProjectResultRowDto>(
                "PMTool_v2.sp_save_project",
                p,
                commandType: CommandType.StoredProcedure
            );

            string message       = p.Get<string>("@msg")          ?? string.Empty;
            int    returnedId    = p.Get<int>("@ReturnProjectId");
            bool   success       = returnedId > 0;

            return new SaveProjectResponseDto
            {
                Message   = message,
                Success   = success,
                ProjectId = returnedId
            };
        }
    }
}
