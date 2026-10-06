namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class AddTaskCommentResponseDto
    {
        public int Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int Comment_Id { get; set; }

        public int Task_Id { get; set; }

        public int Employee_Id { get; set; }

        public string Comment_Type { get; set; } = string.Empty;

        public string Comment_Text { get; set; } = string.Empty;

        public decimal? Planned_Hours { get; set; }

        public decimal? Actual_Hours { get; set; }
    }
}