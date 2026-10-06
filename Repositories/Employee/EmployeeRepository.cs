using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Employee;
using Medtrix_PmTool_V2_Api.DTOs.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Employee;
using Medtrix_PmTool_V2_Api.Services.Auth;
using System.Data;
using EmployeeModel = Medtrix_PmTool_V2_Api.Models.Employee.Employee;

namespace Medtrix_PmTool_V2_Api.Repositories.Employee
{
    public class EmployeeRepository : IEmployee
    {
        private readonly DbContext.DbContext _dbContext;
        private readonly IForgotPassword _forgotPassword;

        public EmployeeRepository(DbContext.DbContext dbContext, IForgotPassword forgotPassword)
        {
            _dbContext = dbContext;
            _forgotPassword = forgotPassword;
        }

        public async Task<IEnumerable<EmployeeTotalBindData>> GetAllEmployeesAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<EmployeeTotalBindData>(
                "sp_bind_all_employees_data",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<EmployeeNameAndIdDto>> GetAllEmployeeNameAndIdAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            var employee = await connection.QueryAsync<EmployeeModel>(
                "sp_bind_all_employees_data",
                commandType: CommandType.StoredProcedure
            );

            if (employee == null) return null;

            return employee.Select(e => new EmployeeNameAndIdDto
            {
                EmployeeId = e.EmpId,
                EmployeeName = e.Employee_Name
            }).ToList();
        }

        // GET /api/v1/employee/types
        public async Task<IEnumerable<UserTypeDto>> GetEmpTypesAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                "SP_BindTeamsDrpdwn",
                commandType: CommandType.StoredProcedure
            );
            await multi.ReadAsync<dynamic>();
            return await multi.ReadAsync<UserTypeDto>();
        }

        // GET /api/v1/employee/sub-teams?teamId=2
        public async Task<IEnumerable<SubTeamDto>> GetSubTeamsAsync(int teamId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<SubTeamDto>(
                "Sp_BindSub_TeamDrpDownList",
                new { TeamId = teamId },
                commandType: CommandType.StoredProcedure
            );
        }

        // GET /api/v1/employee/project-details?empId=5&searchText=pfizer&pageNumber=1&pageSize=10
        public async Task<EmployeeProjectDetailResponseDto> GetEmployeeProjectDetailsAsync(int empId, string? searchText, int pageNumber, int pageSize)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",      empId,      DbType.Int32);
            parameters.Add("@search_text", searchText, DbType.String);
            parameters.Add("@page_number", pageNumber, DbType.Int32);
            parameters.Add("@page_size",   pageSize,   DbType.Int32);
            parameters.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var projects = (await connection.QueryAsync<EmployeeProjectDetailDto>(
                "PMTool_v2.sp_admin_get_emp_projects_details",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            int totalCount = parameters.Get<int>("@total_count");
            int totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

            return new EmployeeProjectDetailResponseDto
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize   = pageSize,
                TotalPages = totalPages,
                Projects   = projects
            };
        }

        // GET /api/v1/employee/details?empId=5
        public async Task<EmployeeDetailsByIdResponseDto> GetEmployeeDetailsByIdAsync(int empId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@empid", empId, DbType.Int32);

            using var multi = await connection.QueryMultipleAsync(
                "sp_get_emp_details_by_id",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int totalTasks    = await multi.ReadSingleOrDefaultAsync<int>();
            int totalProjects = await multi.ReadSingleOrDefaultAsync<int>();
            var empInfo       = await multi.ReadFirstOrDefaultAsync<EmployeeInfoDto>();

            return new EmployeeDetailsByIdResponseDto
            {
                TotalTasks    = totalTasks,
                TotalProjects = totalProjects,
                EmployeeInfo  = empInfo
            };
        }

        // POST /api/v1/employee
        public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(
    CreateEmployeeRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            string password = request.Password;

            if (string.IsNullOrWhiteSpace(password))
            {
                password = $"Temp@{Random.Shared.Next(100000, 999999)}";
            }

            var parameters = new DynamicParameters();

            parameters.Add("@empNo", request.EmpNo, DbType.String);
            parameters.Add("@team_id", request.TeamId, DbType.Int32);
            parameters.Add("@username", request.Username, DbType.String);
            parameters.Add("@emailid", request.EmailId, DbType.String);

            parameters.Add(
                "@password",
                TokenService.HashPassword(password),
                DbType.String
            );

            parameters.Add("@designation", request.Designation, DbType.String);
            parameters.Add("@UserType", request.UserType, DbType.Int32);
            parameters.Add("@empDesigType", request.EmpDesigType, DbType.Int32);
            parameters.Add("@EmpHours", request.EmpHours, DbType.Int32);
            parameters.Add("@Shortdesignation", request.ShortDesignation, DbType.String);
            parameters.Add("@IsUtilRequired", request.IsUtilRequired, DbType.Int32);

            parameters.Add(
                "@msg",
                dbType: DbType.String,
                size: 50,
                direction: ParameterDirection.Output
            );

            await connection.ExecuteAsync(
                "SP_InsertRegisteredUsers",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            string msg = parameters.Get<string>("@msg") ?? string.Empty;

            Console.WriteLine($"Create Employee Message: {msg}");

            if (msg == "Added")
            {
                await _forgotPassword.GenerateOtpAsync(
                    new ForgotPasswordRequest
                    {
                        Email = request.EmailId
                    });
            }

            return new CreateEmployeeResponseDto
            {
                Message = msg
            };
        }

        public async Task<HashPasswordMigrationResponseDto> HashExistingPasswordsAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var employees = await connection.QueryAsync<dynamic>(
                    @"SELECT EmpId, Password
              FROM Tbl_Employee",
                    transaction: transaction);

                int updated = 0;
                int skipped = 0;

                foreach (var employee in employees)
                {
                    string password = employee.Password;

                    // Already hashed
                    if (!string.IsNullOrEmpty(password) && password.Length == 45)
                    {
                        skipped++;
                        continue;
                    }

                    string hashedPassword = TokenService.HashPassword(password);

                    await connection.ExecuteAsync(
                        @"UPDATE Tbl_Employee
                  SET Password = @Password
                  WHERE EmpId = @EmpId",
                        new
                        {
                            EmpId = employee.EmpId,
                            Password = hashedPassword
                        },
                        transaction);

                    updated++;
                }

                transaction.Commit();

                return new HashPasswordMigrationResponseDto
                {
                    Success = true,
                    TotalEmployees = employees.Count(),
                    UpdatedCount = updated,
                    SkippedCount = skipped,
                    FailedCount = 0,
                    Message = "Password migration completed successfully."
                };
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        // PUT /api/v1/employee/{empId}
        public async Task<UpdateEmployeeResponseDto> UpdateEmployeeAsync(int empId, UpdateEmployeeRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@empname",        request.EmpName,        DbType.String);
            parameters.Add("@email",          request.Email,          DbType.String);
            parameters.Add("@designation",    request.Designation,    DbType.String);
            parameters.Add("@team",           request.Team,           DbType.Int32);
            parameters.Add("@empid",          empId,                  DbType.Int32);
            parameters.Add("@empType",        request.EmpType,        DbType.Int32);
            parameters.Add("@is_active_val",  request.IsActive,       DbType.Int32);
            parameters.Add("@EmpHours",       request.EmpHours,       DbType.Int32);
            parameters.Add("@Shortdesignation", request.ShortDesignation, DbType.String);
            parameters.Add("@IsUtilRequired", request.IsUtilRequired, DbType.Int32);
            parameters.Add("@msg", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "SP_empDetails",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            string msg = parameters.Get<string>("@msg") ?? string.Empty;

            if (msg == "Updated")
            {
                await connection.ExecuteAsync(
                    $"INSERT INTO [dbo].[Tbl_UserActivities] VALUES('Updated Employee','Updated',GETDATE(),{request.UpdatedBy})"
                );
            }

            return new UpdateEmployeeResponseDto { Message = msg };
        }
    }
}
