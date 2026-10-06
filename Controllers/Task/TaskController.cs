using Medtrix_PmTool_V2_Api.DTOs.Task;
using Medtrix_PmTool_V2_Api.DTOs.Project;
using Medtrix_PmTool_V2_Api.Interfaces.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Medtrix_PmTool_V2_Api.Controllers.Task
{
    [Authorize(Policy = "AllUsers")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITask _taskRepository;

        public TaskController(ITask taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET /api/task?empId=5&userType=1&startDate=2026-07-01&endDate=2026-07-11
        [HttpGet]
        public async Task<IActionResult> GetTaskList(
            [FromQuery] int empId,
            [FromQuery] int userType,
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string? searchText = null,
            [FromQuery] int? projectId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var result = await _taskRepository.GetTaskListAsync(new TaskListRequestDto
            {
                EmpId      = empId,
                UserType   = userType,
                StartDate  = startDate,
                EndDate    = endDate,
                SearchText = searchText,
                ProjectId  = projectId,
                PageNumber = pageNumber,
                PageSize   = pageSize
            });
            return Ok(result);
        }

        // GET /api/task/projects?empId=5&userType=1
        [HttpGet("projects")]
        public async Task<IActionResult> GetProjectDropdown(
            [FromQuery] int empId,
            [FromQuery] int userType)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            var result = await _taskRepository.GetProjectDropdownAsync(new TaskProjectDropdownRequestDto
            {
                EmpId    = empId,
                UserType = userType
            });
            return Ok(result);
        }

        // GET /api/task/project-filter?empId=5&userType=1&startDate=2026-07-01&endDate=2026-07-11
        [HttpGet("project-filter")]
        public async Task<IActionResult> GetProjectFilter(
            [FromQuery] int empId,
            [FromQuery] int userType,
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            if (empId <= 0) return BadRequest("empId is required.");
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                return BadRequest("startDate and endDate are required.");

            var result = await _taskRepository.GetProjectFilterAsync(new TaskListRequestDto
            {
                EmpId     = empId,
                UserType  = userType,
                StartDate = startDate,
                EndDate   = endDate
            });
            return Ok(result);
        }

        // GET /api/task/by-project?projectId=2&empId=5&userType=1&pageNumber=1&pageSize=10
        [HttpGet("by-project")]
        public async Task<IActionResult> GetTasksByProjectId(
            [FromQuery] int projectId,
            [FromQuery] int empId,
            [FromQuery] int userType,
            [FromQuery] string? searchText = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");
            if (empId <= 0)     return BadRequest("empId is required.");

            var result = await _taskRepository.GetTasksByProjectIdAsync(new TaskByProjectRequestDto
            {
                ProjectId  = projectId,
                EmpId      = empId,
                UserType   = userType,
                SearchText = searchText,
                PageNumber = pageNumber,
                PageSize   = pageSize
            });
            return Ok(result);
        }

        // GET /api/task/cost-info?projectId=5636&buyingCenterId=8
        [HttpGet("cost-info")]
        public async Task<IActionResult> GetProjectCostInfo(
            [FromQuery] int projectId,
            [FromQuery] int buyingCenterId = 0)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");
            var result = await _taskRepository.GetProjectCostInfoAsync(projectId, buyingCenterId);
            return Ok(result);
        }

        // GET /api/task/form-data?projectId=5636&templateId=0&buyingCenterId=8
        [HttpGet("form-data")]
        public async Task<IActionResult> GetTaskFormData(
            [FromQuery] int projectId,
            [FromQuery] int templateId = 0,
            [FromQuery] int? buyingCenterId = null)
        {
            if (projectId <= 0) return BadRequest("projectId is required.");
            var result = await _taskRepository.GetTaskFormDataAsync(new TaskFormDataRequestDto
            {
                ProjectId     = projectId,
                TemplateId    = templateId,
                BuyingCenterId = buyingCenterId
            });
            return Ok(result);
        }

        // GET /api/task/{taskId}
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById([FromRoute] int taskId)
        {
            if (taskId <= 0) return BadRequest("Invalid taskId.");
            var result = await _taskRepository.GetTaskByIdAsync(taskId);
            return Ok(result);
        }

        // POST /api/task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDto request)
        {
            if (request.ProjectId <= 0)                    return BadRequest("projectId is required.");
            if (string.IsNullOrWhiteSpace(request.TaskTitle)) return BadRequest("taskTitle is required.");
            if (request.CreatedBy <= 0)                    return BadRequest("createdBy is required.");

            var result = await _taskRepository.CreateTaskAsync(request);
            return Ok(result);
        }

        // PUT /api/task/{taskId}
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] int taskId, [FromBody] UpdateTaskRequestDto request)
        {
            if (taskId <= 0)                               return BadRequest("Invalid taskId.");
            if (string.IsNullOrWhiteSpace(request.TaskTitle)) return BadRequest("taskTitle is required.");
            if (request.UpdatedBy <= 0)                    return BadRequest("updatedBy is required.");

            var result = await _taskRepository.UpdateTaskAsync(taskId, request);
            return Ok(result);
        }

        // DELETE /api/task/{taskId}
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int taskId, [FromBody] TaskDeleteRequestDto body)
        {
            if (taskId <= 0)         return BadRequest("Invalid taskId.");
            if (body.DeletedBy <= 0) return BadRequest("deletedBy is required.");

            var result = await _taskRepository.DeleteTaskAsync(taskId, body);

            if (result == null || result.TaskId == 0)
                return NotFound(new { message = result?.Message ?? "Task not found." });

            return Ok(new { message = result.Message, taskId = result.TaskId });
        }

        // POST /api/v1/task/reschedule
        [HttpPost("reschedule")]
        public async Task<IActionResult> RescheduleTask(
            [FromBody] RescheduleTaskRequestDto request)
        {
            if (request.TaskId <= 0)
                return BadRequest("taskId is required.");

            if (request.EmployeeId <= 0)
                return BadRequest("employeeId is required.");

            if (request.OriginalDate == default)
                return BadRequest("originalDate is required.");

            if (request.RescheduleDate == default)
                return BadRequest("rescheduleDate is required.");

            if (request.RescheduleDate.Date <= request.OriginalDate.Date)
                return BadRequest("rescheduleDate must be after originalDate.");

            if (string.IsNullOrWhiteSpace(request.RescheduleType))
                return BadRequest("rescheduleType is required.");

            if (request.RescheduleType != "FULL_TASK" &&
                request.RescheduleType != "REMAINING_HOURS")
            {
                return BadRequest(
                    "rescheduleType must be FULL_TASK or REMAINING_HOURS.");
            }

            if (request.CreatedBy <= 0)
                return BadRequest("createdBy is required.");

            var result = await _taskRepository.RescheduleTaskAsync(request);

            if (result == null || result.Success == 0)
                return BadRequest(result);

            return Ok(result);
        }

        // POST /api/v1/Task/comment
        [HttpPost("comment")]
        public async Task<IActionResult> AddTaskComment(
            [FromBody] AddTaskCommentRequestDto request)
        {
            if (request.TaskId <= 0)
                return BadRequest("taskId is required.");

            if (request.EmployeeId <= 0)
                return BadRequest("employeeId is required.");

            if (string.IsNullOrWhiteSpace(request.CommentType))
                return BadRequest("commentType is required.");

            if (string.IsNullOrWhiteSpace(request.CommentText))
                return BadRequest("commentText is required.");

            if (request.CreatedBy <= 0)
                return BadRequest("createdBy is required.");

            var result = await _taskRepository.AddTaskCommentAsync(request);

            if (result == null || result.Success == 0)
                return BadRequest(result?.Message ?? "Unable to add task comment.");

            return Ok(result);
        }

        // GET /api/v1/Task/{taskId}/comments
        [HttpGet("{taskId}/comments")]
        public async Task<IActionResult> GetTaskComments(
            [FromRoute] int taskId)
        {
            if (taskId <= 0)
                return BadRequest("Invalid taskId.");

            var result = await _taskRepository.GetTaskCommentsAsync(taskId);

            return Ok(result);
        }

        // PUT /api/v1/Task/comments/{commentId}/read
        [HttpPut("comments/{commentId}/read")]
        public async Task<IActionResult> MarkTaskCommentRead(
            [FromRoute] int commentId)
        {
            if (commentId <= 0)
                return BadRequest("Invalid commentId.");

            var result = await _taskRepository.MarkTaskCommentReadAsync(commentId);

            if (result == null || result.Success == 0)
                return BadRequest(result?.Message ?? "Unable to mark comment as read.");

            return Ok(result);
        }

        // GET /api/v1/Task/notifications?recipientId=1180&unreadOnly=true
        [HttpGet("notifications")]
        public async Task<IActionResult> GetTaskNotifications(
            [FromQuery] int recipientId,
            [FromQuery] bool unreadOnly = true)
        {
            if (recipientId <= 0)
                return BadRequest("recipientId is required.");

            var result = await _taskRepository.GetTaskNotificationsAsync(
                recipientId,
                unreadOnly);

            return Ok(result);
        }

        // PUT /api/v1/Task/notifications/{notificationId}/read
        [HttpPut("notifications/{notificationId}/read")]
        public async Task<IActionResult> MarkTaskNotificationRead(
            [FromRoute] int notificationId)
        {
            if (notificationId <= 0)
                return BadRequest("Invalid notificationId.");

            var result = await _taskRepository.MarkTaskNotificationReadAsync(
                notificationId);

            if (result == null || result.Success == 0)
                return BadRequest(
                    result?.Message ?? "Unable to mark notification as read.");

            return Ok(result);
        }

        [HttpGet("template-list")]
        public async Task<IActionResult> GetTaskTemplateList()
        {
            try
            {
                var result = await _taskRepository.GetTaskTemplateListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        message = "Failed to get task template list.",
                        detail = ex.Message
                    }
                );
            }
        }

        [HttpPost("template")]
        public async Task<IActionResult> CreateTaskTemplate(
    [FromBody] CreateTaskTemplateRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        message = "Request data is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.TemplateName))
                {
                    return BadRequest(new
                    {
                        message = "Template name is required."
                    });
                }

                var result =
                    await _taskRepository.CreateTaskTemplateAsync(request);

                return Ok(result);
            }
            catch (SqlException ex)
            {
                return StatusCode(503, new
                {
                    statusCode = 503,
                    message = "Database error. Please try again later.",
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Failed to create task template.",
                    detail = ex.Message
                });
            }
        }
    }
}
