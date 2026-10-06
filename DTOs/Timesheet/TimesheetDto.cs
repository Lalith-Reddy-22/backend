namespace Medtrix_PmTool_V2_Api.DTOs.Timesheet
{
    // ─── REQUEST ─────────────────────────────────────────────────────────────
    public class TimesheetRequestDto
    {
        public int EmpId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    // ─── LOG UPSERT REQUEST ───────────────────────────────────────────────────
    public class AddTimeLogRequestDto
    {
        public int TimeLogId { get; set; } = 0;      // 0 = insert, >0 = update
        public int TaskId { get; set; }
        public int EmpId { get; set; }
        public string LogDate { get; set; } = string.Empty;
        public double LogHours { get; set; }
        public string? Notes { get; set; }
        // BillingType is always 1 — not exposed to caller
    }

    // ─── RAW: Result set 1 — task list ───────────────────────────────────────
    public class RawTaskDto
    {
        public int Task_Id { get; set; }
        public string Task_Title { get; set; } = string.Empty;
        public string tskpriority { get; set; } = string.Empty;

        public int BuyingCenterId { get; set; }

        public string Taskowner { get; set; } = string.Empty;
    }

    // ─── RAW: Result set 2 — log/allocation detail rows ──────────────────────
    public class RawLogDetailDto
    {
        public int Task_Id { get; set; }
        public string status { get; set; } = string.Empty;
        public decimal Working_hours { get; set; }
        public TimeSpan AllocStartTime { get; set; }
        public TimeSpan AllocEndTime { get; set; }
        public string Allocated_Date { get; set; } = string.Empty;
        public decimal Loghours { get; set; }
        public int LogId { get; set; }
    }

    // ─── RESPONSE: per-date entry under each task ─────────────────────────────
    public class TimesheetDateEntryDto
    {
        public string Allocated_Date { get; set; } = string.Empty;
        public decimal Working_hours { get; set; }
        public string AllocStartTime { get; set; } = string.Empty;  // "HH:mm"
        public string AllocEndTime { get; set; } = string.Empty;    // "HH:mm"
        public int LogId { get; set; }
        public decimal Loghours { get; set; }
        public string status { get; set; } = string.Empty;
    }

    // ─── RESPONSE: task row with grouped dates ────────────────────────────────
    public class TimesheetTaskDto
    {
        public int Task_Id { get; set; }
        public string Task_Title { get; set; } = string.Empty;
        public string tskpriority { get; set; } = string.Empty;

        public int BuyingCenterId { get; set; }

        public string Taskowner { get; set; } = string.Empty;
        public List<TimesheetDateEntryDto> dates { get; set; } = new();
    }

    // ─── FINAL RESPONSE ───────────────────────────────────────────────────────
    public class TimesheetResponseDto
    {
        public List<TimesheetTaskDto> tasks { get; set; } = new();
    }
}
