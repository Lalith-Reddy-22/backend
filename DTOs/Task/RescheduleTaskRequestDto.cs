namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class RescheduleTaskRequestDto
    {
        public int TaskId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime OriginalDate { get; set; }

        public DateTime RescheduleDate { get; set; }

        public string RescheduleType { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public int CreatedBy { get; set; }
    }
}