namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class GetTaskCommentsResponseDto
    {
        public int Comment_Id { get; set; }

        public int Task_Id { get; set; }

        public int Employee_Id { get; set; }

        public string Employee_Name { get; set; } = string.Empty;

        public string Comment_Type { get; set; } = string.Empty;

        public string Comment_Text { get; set; } = string.Empty;

        public decimal? Planned_Hours { get; set; }

        public decimal? Actual_Hours { get; set; }

        public int Created_By { get; set; }

        public DateTime Created_Date { get; set; }

        public bool Is_Read { get; set; }
    }
}