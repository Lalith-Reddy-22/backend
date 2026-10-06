namespace Medtrix_PmTool_V2_Api.DTOs.Ticket
{
    // GET /ticket/list
    public class GetTicketListRequestDto
    {
        public int EmpId { get; set; }
        public int EmpType { get; set; } = 1;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? SearchText { get; set; }
        public int? StatusId { get; set; }
        public int? SeverityId { get; set; }
        public int? IssueTypeId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class TicketListItemDto
    {
        public int Issue_Id { get; set; }
        public string Issue_Type { get; set; }
        public string Subcategory { get; set; }
        public string Severity { get; set; }
        public string Issue_Description { get; set; }
        public string Status { get; set; }
        public string Issue_Date { get; set; }
        public string Employee_Name { get; set; }
        public string EmailId { get; set; }
        public string Resolved_Date_Formatted { get; set; }
        public string Comments { get; set; }
        public string Error { get; set; }
        public string Status_Text { get; set; }
        public int Days_Open { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class GetTicketListResponseDto
    {
        public IEnumerable<TicketListItemDto> Tickets { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    // GET /ticket/form-data
    public class TicketStatusDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class TicketSeverityDto
    {
        public int Id { get; set; }
        public string Severity { get; set; }
    }

    public class TicketIssueTypeDto
    {
        public int Id { get; set; }
        public string Issue_Type { get; set; }
    }

    public class TicketEmployeeDto
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; }
    }

    public class TicketFormDataResponseDto
    {
        public IEnumerable<TicketStatusDto> Statuses { get; set; }
        public IEnumerable<TicketSeverityDto> Severities { get; set; }
        public IEnumerable<TicketIssueTypeDto> IssueTypes { get; set; }
        public IEnumerable<TicketEmployeeDto> Employees { get; set; }
    }

    // POST /ticket
    public class CreateTicketRequestDto
    {
        public int IssueType { get; set; }
        public int SubIssueType { get; set; }
        public int Severity { get; set; }
        public string IssueDescription { get; set; }
        public int EmpId { get; set; }
        public int Status { get; set; } = 1;
    }

    public class CreateTicketResponseDto
    {
        public int IssueId { get; set; }
        public string Message { get; set; }
    }

    // PUT /ticket/update-status
    public class UpdateTicketStatusRequestDto
    {
        public int IssueId { get; set; }
        public int Status { get; set; }
        public string? Comments { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class UpdateTicketStatusResponseDto
    {
        public int Issue_Id { get; set; }
        public string Issue_Type { get; set; }
        public string Subcategory { get; set; }
        public string Severity { get; set; }
        public string Issue_Description { get; set; }
        public string Status { get; set; }
        public string Status_Text { get; set; }
        public string Issue_Date { get; set; }
        public string Resolved_Date { get; set; }
        public string Comments { get; set; }
        public int Days_Open { get; set; }
        public string Message { get; set; }
    }
}
