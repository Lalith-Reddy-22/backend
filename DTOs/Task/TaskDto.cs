namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    // ─── REQUEST: task list + project filter ─────────────────────────────────
    public class TaskListRequestDto
    {
        public int EmpId { get; set; }
        public int UserType { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string? SearchText { get; set; }
        public int? ProjectId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ─── REQUEST: projects dropdown ──────────────────────────────────────────
    public class TaskProjectDropdownRequestDto
    {
        public int EmpId { get; set; }
        public int UserType { get; set; }
    }

    // ─── REQUEST: tasks by project id ────────────────────────────────────────
    public class TaskByProjectRequestDto
    {
        public int EmpId { get; set; }
        public int UserType { get; set; }
        public int ProjectId { get; set; }
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ─── RESULT: Paginated task row ───────────────────────────────────────────
    public class TaskRowDto
    {
        public int Task_Id { get; set; }
        public string Task_Unique_ID { get; set; } = string.Empty;
        public string Task_Title { get; set; } = string.Empty;
        public string tskpriority { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string Status_Text { get; set; } = string.Empty;
        public string start_date { get; set; } = string.Empty;
        public string end_date { get; set; } = string.Empty;
        public decimal Working_hours { get; set; }
        public int ProjectId { get; set; }
        public int ClientID { get; set; }
        public int Create_By { get; set; }
        public int tempId { get; set; }
        public string Created_By_Name { get; set; } = string.Empty;
        public string projectCode { get; set; } = string.Empty;
        public string project_Title { get; set; } = string.Empty;
        public int project_ID { get; set; }
        public int BuyingCenterId { get; set; }
        public string BuyingCenterName { get; set; } = string.Empty;
        public string Employee_Names { get; set; } = string.Empty;
        public string Start_Date_Formatted { get; set; } = string.Empty;
        public string End_Date_Formatted { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    // ─── RESULT: Distinct projects for filter ────────────────────────────────
    public class TaskProjectFilterDto
    {
        public int project_ID { get; set; }
        public string project_Title { get; set; } = string.Empty;
        public string projectCode { get; set; } = string.Empty;
    }

    // ─── RESULT: Projects for dropdown ───────────────────────────────────────
    public class TaskProjectDropdownDto
    {
        public int project_ID { get; set; }
        public string project_Title { get; set; } = string.Empty;
        public int B_Id { get; set; }
        public string sel_id { get; set; } = string.Empty;
    }

    // ─── API 1 RESPONSE: paginated tasks ─────────────────────────────────────
    public class TaskListResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<TaskRowDto> Tasks { get; set; } = Enumerable.Empty<TaskRowDto>();
    }

    // ─── API 2 RESPONSE: projects dropdown ───────────────────────────────────
    public class TaskProjectDropdownResponseDto
    {
        public IEnumerable<TaskProjectDropdownDto> Projects { get; set; } = Enumerable.Empty<TaskProjectDropdownDto>();
    }

    // ─── API 3 RESPONSE: distinct project filter ─────────────────────────────
    public class TaskProjectFilterResponseDto
    {
        public IEnumerable<TaskProjectFilterDto> ProjectFilter { get; set; } = Enumerable.Empty<TaskProjectFilterDto>();
    }

    // ─── API 4 REQUEST: delete task ──────────────────────────────────────────
    public class TaskDeleteRequestDto
    {
        public int DeletedBy { get; set; }
        public string DeleteReason { get; set; } = string.Empty;
    }

    // ─── API 4 RESPONSE: delete task ─────────────────────────────────────────
    public class TaskDeleteResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int TaskId { get; set; }
    }

    // ─── API 5 REQUEST: form data ────────────────────────────────────────────
    public class TaskFormDataRequestDto
    {
        public int ProjectId { get; set; }
        public int TemplateId { get; set; }
        public int? BuyingCenterId { get; set; }
    }

    // ─── API 5 RESPONSE: form data ───────────────────────────────────────────
    public class TaskFormDataResponseDto
    {
        public string ClientName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public IEnumerable<TaskStatusDto> Statuses { get; set; } = Enumerable.Empty<TaskStatusDto>();
        public IEnumerable<TaskEmployeeDto> Employees { get; set; } = Enumerable.Empty<TaskEmployeeDto>();
        public TaskTemplateInfoDto? TemplateInfo { get; set; }
    }

    public class TaskStatusDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TaskEmployeeDto
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
    }

    public class TaskTemplateInfoDto
    {
        public string TaskName { get; set; } = string.Empty;
        public string ActualHours { get; set; } = string.Empty;
        public string Responsibility { get; set; } = string.Empty;
    }

    // ─── API 6 RESPONSE: edit task ───────────────────────────────────────────
    public class EditTaskResponseDto
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string EmpIds { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Status { get; set; }
        public string TaskPriority { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public int TemplateId { get; set; }
        public IEnumerable<EmployeeAllocationDto> EmployeeAllocations { get; set; } = Enumerable.Empty<EmployeeAllocationDto>();
        public TaskTemplateInfoDto? TemplateInfo { get; set; }
    }

    // ─── API 7 REQUEST: create task ──────────────────────────────────────────
    public class CreateTaskRequestDto
    {
        public string TaskTitle { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public int ClientId { get; set; }
        public int TemplateId { get; set; } = 0;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public List<EmployeeAllocationDto> EmployeeAllocations { get; set; } = new();
    }

    // ─── API 8 REQUEST: update task ──────────────────────────────────────────
    public class UpdateTaskRequestDto
    {
        public string TaskTitle { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public int ClientId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public int UpdatedBy { get; set; }
        public string OriginalEmpIds { get; set; } = string.Empty;
        public List<EmployeeAllocationDto> EmployeeAllocations { get; set; } = new();
    }

    // ─── PER-EMPLOYEE ALLOCATION ──────────────────────────────────────────────
    public class EmployeeAllocationDto
    {
        public int EmployeeId { get; set; }
        public List<DailyAllocationDto> DailyAllocations { get; set; } = new();
    }

    // ─── PER-DAY ALLOCATION SLOT ─────────────────────────────────────────────
    public class DailyAllocationDto
    {
        public string AllocationDate { get; set; } = string.Empty;
        public float Hours { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }

    // ─── API 7 & 8 RESPONSE ──────────────────────────────────────────────────
    public class CreateTaskResponseDto
    {
        public int TaskId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // ─── TASKS BY PROJECT RESPONSE ───────────────────────────────────────────
    public class TaskByProjectResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<TaskRowDto> Tasks { get; set; } = Enumerable.Empty<TaskRowDto>();
    }

    // ─── TASK TEMPLATE LIST API ─────────────────────────────────────────────

    public class TaskTemplateListDto
    {
        public string TaskName { get; set; } = string.Empty;
        public string ActualHours { get; set; } = string.Empty;
        public string Responsibility { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public int TempId { get; set; }
        public int TaskId { get; set; }
    }

    public class TaskTemplateDto
    {
        public string TemplateName { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    public class TaskTemplateProjectDto
    {
        public string Project_Title { get; set; } = string.Empty;
        public int Project_ID { get; set; }
        public int B_Id { get; set; }
        public string Sel_Id { get; set; } = string.Empty;
    }

    public class TaskTemplateListResponseDto
    {
        public IEnumerable<TaskTemplateListDto> Tasks { get; set; }
            = Enumerable.Empty<TaskTemplateListDto>();

        public IEnumerable<TaskTemplateDto> Templates { get; set; }
            = Enumerable.Empty<TaskTemplateDto>();

        public IEnumerable<TaskTemplateProjectDto> Projects { get; set; }
            = Enumerable.Empty<TaskTemplateProjectDto>();
    }

    // ─── CREATE TASK TEMPLATE REQUEST ───────────────────────────────
    public class CreateTaskTemplateRequestDto
    {
        public string TemplateName { get; set; } = string.Empty;

        public string TaskDetails { get; set; } = string.Empty;

        public string TaskName { get; set; } = string.Empty;

        public string Responsibilities { get; set; } = string.Empty;

        public string ActualHours { get; set; } = string.Empty;

        public int CreateBy { get; set; }
    }

    // ─── CREATE TASK TEMPLATE RESPONSE ──────────────────────────────
    public class CreateTaskTemplateResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
