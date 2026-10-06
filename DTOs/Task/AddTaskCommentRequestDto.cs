namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class AddTaskCommentRequestDto
    {
        public int TaskId { get; set; }

        public int EmployeeId { get; set; }

        public string CommentType { get; set; } = string.Empty;

        public string CommentText { get; set; } = string.Empty;

        public decimal? PlannedHours { get; set; }

        public decimal? ActualHours { get; set; }

        public int CreatedBy { get; set; }
    }
}