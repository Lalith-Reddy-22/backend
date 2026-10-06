namespace Medtrix_PmTool_V2_Api.DTOs.Leave
{
    // Dropdown DTOs
    public class TeamDropdownDto
    {
        public int Id { get; set; }
        public string Teams_Name { get; set; } = string.Empty;
    }

    public class DesignationDropdownDto
    {
        public string EmpDesignation { get; set; } = string.Empty;
    }

    // Leave list with pagination support
    public class LeaveListDto
    {
        public int TotalRecords { get; set; }
        public int Id { get; set; }
        public int Team_Id { get; set; }
        public string Teams_Name { get; set; } = string.Empty;
        public decimal Design_Billablevalue { get; set; }
        public decimal Design_NonBillablevalue { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string Start_date { get; set; } = string.Empty;
        public string End_date { get; set; } = string.Empty;
    }

    public class LeaveListRequestDto
    {
        public string? TeamName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // Get by ID (for edit pre-fill)
    public class LeaveDetailDto
    {
        public int Id { get; set; }
        public int Team_id { get; set; }
        public string Designation { get; set; } = string.Empty;
        public decimal Design_Billablevalue { get; set; }
        public decimal Design_NonBillablevalue { get; set; }
        public string Start_date { get; set; } = string.Empty;
        public string End_date { get; set; } = string.Empty;
    }

    // Create request (no ID needed)
    public class CreateLeaveRequestDto
    {
        public int TeamId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public int BillableLeaveHours { get; set; }
        public int NonBillableLeaveHours { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    // Update request (ID comes from route)
    public class UpdateLeaveRequestDto
    {
        public int TeamId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public int BillableLeaveHours { get; set; }
        public int NonBillableLeaveHours { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }
}
