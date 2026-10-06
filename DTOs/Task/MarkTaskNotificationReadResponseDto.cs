namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class MarkTaskNotificationReadResponseDto
    {
        public int Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int Notification_Id { get; set; }
    }
}