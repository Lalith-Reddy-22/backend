namespace Medtrix_PmTool_V2_Api.DTOs.Employee
{
    public class EmployeeDto { }

    public class EmployeeNameAndIdDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }

    public class EmployeeTotalBindData
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; }
        public string EmailId { get; set; }
        public string Designation { get; set; }
        public string Created_Date { get; set; }
        public string Teams_Name { get; set; }
        public string User_Type { get; set; }
        public string Status { get; set; }
        public string Inital_Letters { get; set; }
    }

    // ─── Emp Types (SP_BindTeamsDrpdwn → Tables[1]) ──────────────────────────
    public class UserTypeDto
    {
        public int Id { get; set; }
        public string User_Type { get; set; } = string.Empty;
    }

    // ─── Sub Teams (Sp_BindSub_TeamDrpDownList) ───────────────────────────────
    public class SubTeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ─── Employee Project Details (sp_admin_get_emp_projects_details) ────────
    public class EmployeeProjectDetailDto
    {
        public int project_Id { get; set; }
        public int client_Id { get; set; }
        public string Client_Title { get; set; } = string.Empty;
        public string project_title { get; set; } = string.Empty;
        public string projectCode { get; set; } = string.Empty;
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int Project_Status { get; set; }
        public string Project_Status_Text { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public decimal Total_Value { get; set; }
        public int Completed_Tasks { get; set; }
        public int Total_Tasks { get; set; }
        public decimal Logged_Hours { get; set; }
        public decimal Allocated_Hours { get; set; }
        public string Start_Date_Formatted { get; set; } = string.Empty;
        public string End_Date_Formatted { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class EmployeeProjectDetailResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<EmployeeProjectDetailDto> Projects { get; set; } = Enumerable.Empty<EmployeeProjectDetailDto>();
    }

    // ─── Employee Details By Id (sp_get_emp_details_by_id) ───────────────────
    public class EmployeeInfoDto
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Teams_Name { get; set; } = string.Empty;
        public string User_Type { get; set; } = string.Empty;
        public int IsActive { get; set; }
        public int Team_Id_val { get; set; }
        public int User_Type_val { get; set; }
        public string EmpDesignation { get; set; } = string.Empty;
        public decimal EmpHours { get; set; }
        public int UtilizationRequired { get; set; }
    }

    public class EmployeeDetailsByIdResponseDto
    {
        public int TotalTasks { get; set; }
        public int TotalProjects { get; set; }
        public EmployeeInfoDto? EmployeeInfo { get; set; }
    }

    // ─── Create Employee (SP_InsertRegisteredUsers) ───────────────────────────
    public class CreateEmployeeRequestDto
    {
        public string EmpNo { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string Designation { get; set; } = string.Empty;
        public int UserType { get; set; }
        public int EmpDesigType { get; set; }
        public int EmpHours { get; set; }
        public string ShortDesignation { get; set; } = string.Empty;
        public int IsUtilRequired { get; set; }
    }

    public class CreateEmployeeResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public bool Success => Message == "Added";
    }

    // ─── Update Employee (SP_empDetails) ─────────────────────────────────────
    public class UpdateEmployeeRequestDto
    {
        public string EmpName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int Team { get; set; }
        public int EmpType { get; set; }
        public int IsActive { get; set; }
        public int EmpHours { get; set; }
        public string ShortDesignation { get; set; } = string.Empty;
        public int IsUtilRequired { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class UpdateEmployeeResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public bool Success => Message == "Updated";
    }
}
