namespace Medtrix_PmTool_V2_Api.DTOs.Reports
{
    public class DailyReportDto
    {
        public int TotalRecords { get; set; }
        public int Task_ID { get; set; }
        public string tskpriority { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public TimeSpan AllocStartTime { get; set; }
        public TimeSpan AllocEndTime { get; set; }
        public string Owner { get; set; } = string.Empty;
        public decimal Working_hours { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
    }

    public class DailyReportRequestDto
    {
        public DateTime? TodayDate { get; set; }

        public int? TaskId { get; set; }

        public string? ResourceName { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public class PagedResult<T>
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    }
}
