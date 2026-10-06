namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class RescheduleTaskResponseDto
    {
        public int Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int Task_Id { get; set; }

        public int Employee_Id { get; set; }

        public DateTime Original_Date { get; set; }

        public DateTime Reschedule_Date { get; set; }

        public decimal Planned_Hours { get; set; }

        public decimal Completed_Hours { get; set; }

        public decimal Remaining_Hours { get; set; }

        public string Reschedule_Type { get; set; } = string.Empty;
    }
}