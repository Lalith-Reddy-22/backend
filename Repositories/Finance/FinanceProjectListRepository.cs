using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Finance
{
    public class FinanceProjectListRepository : IFinanceProjectList
    {
        private readonly DbContext.DbContext _db;

        public FinanceProjectListRepository(DbContext.DbContext db)
        {
            _db = db;
        }

        // GET /api/v1/finance-project-list?empId=10
        // SP_GetAllProjectList — returns all projects filtered by empId (PM or admin)
        public async Task<FinanceProjectListResponseDto> GetProjectListAsync(int empId)
        {
            using var conn = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Emp_Id", empId, DbType.Int32);

            var projects = (await conn.QueryAsync<FinanceProjectListItemDto>(
                "SP_GetAllProjectList",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            return new FinanceProjectListResponseDto
            {
                EmpId      = empId,
                TotalCount = projects.Count,
                Projects   = projects
            };
        }
    }
}
