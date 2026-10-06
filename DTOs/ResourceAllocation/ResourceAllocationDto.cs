namespace Medtrix_PmTool_V2_Api.DTOs.ResourceAllocation
{
    // ─── REQUEST ─────────────────────────────────────────────────────────────
    public class ResourceAllocationRequestDto
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int? TeamId { get; set; }                // nullable — SP allows NULL
    }

    // ─── TABLE 1: Employee totals ─────────────────────────────────────────────
    public class RawEmpTotalDto
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
        public double Total_Planned_Hours { get; set; }
        public double Total_Logged_Hours { get; set; }
        public double Daily_Available_Hours { get; set; }
    }

    // ─── TABLE 2: Per-date detail ─────────────────────────────────────────────
    public class RawDailyDetailDto
    {
        public int EmpId { get; set; }
        public string Date { get; set; } = string.Empty;
        public double Planned_Hours { get; set; }
        public double Logged_Hours { get; set; }
        public double Available_Hours { get; set; }
    }

    // ─── RESPONSE: per-date cell ──────────────────────────────────────────────
    public class DateCellDto
    {
        public string date { get; set; } = string.Empty;
        public double planned { get; set; }
        public double logged { get; set; }
        public double available { get; set; }
    }

    // ─── RESPONSE: per-employee row ───────────────────────────────────────────
    public class EmployeeRowDto
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
        public double total_planned { get; set; }
        public double total_logged { get; set; }
        public List<DateCellDto> dates { get; set; } = new();
    }

    // ─── FINAL RESPONSE ───────────────────────────────────────────────────────
    public class ResourceAllocationResponseDto
    {
        public List<EmployeeRowDto> employees { get; set; } = new();
    }
}

    public class EmpTaskDetailRequestDto
    {
        public int EmpId { get; set; }
        public string DateVal { get; set; } = string.Empty;
    }

    public class EmpTaskDetailRowDto
    {
        public int Task_Id { get; set; }
        public string Task_Title { get; set; } = string.Empty;
        public string tskpriority { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public int Project_Id { get; set; }
        public int BuyingCenterId { get; set; }
        public double Working_hours { get; set; }
        public TimeSpan AllocStartTime { get; set; }
        public TimeSpan AllocEndTime { get; set; }
        public string Taskowner { get; set; } = string.Empty;
        public double Logged_hours { get; set; }
    }

    public class EmpTaskHeaderDto
    {
        public string Date { get; set; } = string.Empty;
        public string Employee_Name { get; set; } = string.Empty;
    }

    public class EmpTaskDetailResponseDto
    {
        public EmpTaskHeaderDto? Header { get; set; }
        public IEnumerable<EmpTaskDetailRowDto> Tasks { get; set; } = Enumerable.Empty<EmpTaskDetailRowDto>();
    }
