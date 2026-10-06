using Medtrix_PmTool_V2_Api.DTOs.Employee;

namespace Medtrix_PmTool_V2_Api.Interfaces.Employee
{
    public interface IEmployee
    {
        Task<IEnumerable<EmployeeTotalBindData>> GetAllEmployeesAsync();
        Task<List<EmployeeNameAndIdDto>> GetAllEmployeeNameAndIdAsync();
        Task<EmployeeProjectDetailResponseDto> GetEmployeeProjectDetailsAsync(int empId, string? searchText, int pageNumber, int pageSize);
        Task<EmployeeDetailsByIdResponseDto> GetEmployeeDetailsByIdAsync(int empId);
        Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeRequestDto request);
        Task<UpdateEmployeeResponseDto> UpdateEmployeeAsync(int empId, UpdateEmployeeRequestDto request);
        Task<IEnumerable<UserTypeDto>> GetEmpTypesAsync();
        Task<IEnumerable<SubTeamDto>> GetSubTeamsAsync(int teamId);
        Task<HashPasswordMigrationResponseDto> HashExistingPasswordsAsync();
    }
}
