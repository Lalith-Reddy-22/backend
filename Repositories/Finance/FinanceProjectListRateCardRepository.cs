using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;

using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Finance
{
    public class FinanceProjectListRateCardRepository : IFinanceProjectListRateCard
    {
        private readonly DbContext.DbContext _db;

        public FinanceProjectListRateCardRepository(DbContext.DbContext db)
        {
            _db = db;
        }

        public async Task<List<ProjectDropdownDto>> GetProjectsAsync()
        {
            using var conn = _db.CreateConnection();

            const string sql = @"
                SELECT
                    project_ID AS ProjectId,
                    project_Title AS ProjectName
                FROM Tbl_Project_Details
                ORDER BY project_Title";

            var result = await conn.QueryAsync<ProjectDropdownDto>(sql);

            return result.ToList();
        }

        public async Task<List<EmployeeDropDownDto>> GetEmployeesAsync()
        {
            using var conn = _db.CreateConnection();

            const string sql = @"
        SELECT
            EmpId,
            Employee_Name
        FROM Tbl_Employee
        ORDER BY Employee_Name;";

            var result = await conn.QueryAsync<EmployeeDropDownDto>(sql);

            return result.ToList();
        }
        public async Task<BuyingCenterResponseDto> GetBuyingCenterByProjectAsync(int projectId)
        {
            using var conn = _db.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ProjId", projectId);

            var employees = (await conn.QueryAsync<BuyingCenterEmployeeDto>(
                "SP_BindEmpValByProject",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            var response = new BuyingCenterResponseDto
            {
                Employees = employees
            };

            if (employees.Any())
            {
                response.BuyingCenterName = employees.First().BName;
            }

            return response;
        }

        public async Task<string> CreateProjectRateCardAsync(CreateProjectRateCardRequestDto request)
        {
            using var conn = _db.CreateConnection();

            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                var buyingCenterParameters = new DynamicParameters();

                buyingCenterParameters.Add("@BName", request.Buying_CenterName);
                buyingCenterParameters.Add("@Created_by", request.UserId);
                buyingCenterParameters.Add("@BId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await conn.ExecuteAsync(
                    "SP_BuyCenterInfo",
                    buyingCenterParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                int bId = buyingCenterParameters.Get<int>("@BId");

                if (request.Action != "Update" && bId <= 0)
                {
                    transaction.Rollback();
                    return "Buying Center Name already exists!!!!";
                }

                foreach (var employee in request.EmpSets)
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@EmpId", employee.EmpId);
                    parameters.Add("@ProjId", employee.ProjId);
                    parameters.Add("@EmpVal", employee.EmpValue);
                    parameters.Add("@BId", bId);

                    await conn.ExecuteAsync(
                        "SP_CreateCenterEmpDetails",
                        parameters,
                        transaction,
                        commandType: CommandType.StoredProcedure);
                }

                transaction.Commit();

                return request.Action == "Update"
                    ? "Buying center details updated successfully."
                    : "Buying center details added successfully.";
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<string> UpdateProjectRateCardAsync(
    CreateProjectRateCardRequestDto request)
        {
            using var conn = _db.CreateConnection();

            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                // Get the existing Buying Center ID
                var buyingCenterParameters = new DynamicParameters();

                buyingCenterParameters.Add(
                    "@BName",
                    request.Buying_CenterName);

                buyingCenterParameters.Add(
                    "@Created_by",
                    request.UserId);

                buyingCenterParameters.Add(
                    "@BId",
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                await conn.ExecuteAsync(
                    "SP_BuyCenterInfo",
                    buyingCenterParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                int bId = buyingCenterParameters.Get<int>("@BId");

                if (bId <= 0)
                {
                    transaction.Rollback();

                    return "Buying center not found.";
                }

                // Update employee values
                foreach (var employee in request.EmpSets)
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@EmpId", employee.EmpId);
                    parameters.Add("@ProjId", employee.ProjId);
                    parameters.Add("@EmpVal", employee.EmpValue);
                    parameters.Add("@BId", bId);

                    await conn.ExecuteAsync(
                        "SP_UpdateCenterEmpDetails",
                        parameters,
                        transaction,
                        commandType: CommandType.StoredProcedure);
                }

                transaction.Commit();

                return "Employee rate card details updated successfully.";
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}