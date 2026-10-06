using Medtrix_PmTool_V2_Api.DTOs.Task;
using Medtrix_PmTool_V2_Api.DTOs.Project;

namespace Medtrix_PmTool_V2_Api.Interfaces.Task
{
    public interface ITask
    {
        Task<TaskListResponseDto> GetTaskListAsync(TaskListRequestDto request);
        Task<TaskProjectDropdownResponseDto> GetProjectDropdownAsync(TaskProjectDropdownRequestDto request);
        Task<TaskProjectFilterResponseDto> GetProjectFilterAsync(TaskListRequestDto request);
        Task<TaskDeleteResponseDto> DeleteTaskAsync(int taskId, TaskDeleteRequestDto body);
        Task<TaskFormDataResponseDto> GetTaskFormDataAsync(TaskFormDataRequestDto request);
        Task<EditTaskResponseDto> GetTaskByIdAsync(int taskId);
        Task<CreateTaskResponseDto> CreateTaskAsync(CreateTaskRequestDto request);
        Task<CreateTaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto request);
        Task<TaskByProjectResponseDto> GetTasksByProjectIdAsync(TaskByProjectRequestDto request);
        Task<ProjectCostInfoResponseDto> GetProjectCostInfoAsync(int projectId, int buyingCenterId);
        Task<RescheduleTaskResponseDto> RescheduleTaskAsync(RescheduleTaskRequestDto request);
        Task<AddTaskCommentResponseDto> AddTaskCommentAsync(AddTaskCommentRequestDto request);
        Task<List<GetTaskCommentsResponseDto>> GetTaskCommentsAsync(int taskId);
        Task<MarkTaskCommentReadResponseDto> MarkTaskCommentReadAsync(int commentId);
        Task<List<GetTaskNotificationsResponseDto>> GetTaskNotificationsAsync(int recipientId, bool unreadOnly);
        Task<MarkTaskNotificationReadResponseDto> MarkTaskNotificationReadAsync(int notificationId);
        Task<TaskTemplateListResponseDto> GetTaskTemplateListAsync();
        Task<CreateTaskTemplateResponseDto> CreateTaskTemplateAsync(CreateTaskTemplateRequestDto request);
    }
}
