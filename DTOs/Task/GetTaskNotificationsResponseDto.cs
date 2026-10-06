namespace Medtrix_PmTool_V2_Api.DTOs.Task
{
    public class GetTaskNotificationsResponseDto
    {
        public int Notification_Id { get; set; }

        public int Task_Id { get; set; }

        public int Comment_Id { get; set; }

        public int Employee_Id { get; set; }

        public int Recipient_Id { get; set; }

        public string Notification_Title { get; set; } = string.Empty;

        public string Notification_Message { get; set; } = string.Empty;

        public string Notification_Type { get; set; } = string.Empty;

        public bool Is_Read { get; set; }

        public DateTime Created_Date { get; set; }
    }
}