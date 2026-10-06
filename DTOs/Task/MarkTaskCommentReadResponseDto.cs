namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class MarkTaskCommentReadResponseDto
    {
        public int Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int Comment_Id { get; set; }
    }
}