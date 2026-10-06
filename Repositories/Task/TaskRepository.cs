using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Task;
using Medtrix_PmTool_V2_Api.DTOs.Project;
using Medtrix_PmTool_V2_Api.Interfaces.Task;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Task
{
    public class TaskRepository : ITask
    {
        private readonly DbContext.DbContext _dbContext;

        public TaskRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ─── API 1: sp_get_tasks_list
        public async Task<TaskListResponseDto> GetTaskListAsync(TaskListRequestDto request)
        {
            if (!DateTime.TryParse(request.StartDate, out var startDate))
                throw new ArgumentException($"Invalid startDate: '{request.StartDate}'. Expected yyyy-MM-dd.");
            if (!DateTime.TryParse(request.EndDate, out var endDate))
                throw new ArgumentException($"Invalid endDate: '{request.EndDate}'. Expected yyyy-MM-dd.");

            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",      request.EmpId,      DbType.Int32);
            parameters.Add("@usr_typ",     request.UserType,   DbType.Int32);
            parameters.Add("@start_date",  startDate,          DbType.Date);
            parameters.Add("@end_date",    endDate,            DbType.Date);
            parameters.Add("@search_text", request.SearchText, DbType.String);
            parameters.Add("@project_id",  request.ProjectId,  DbType.Int32);
            parameters.Add("@page_number", request.PageNumber, DbType.Int32);
            parameters.Add("@page_size",   request.PageSize,   DbType.Int32);
            parameters.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var tasks = (await connection.QueryAsync<TaskRowDto>(
                "PMTool_v2.sp_get_tasks_list",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            int totalCount = parameters.Get<int>("@total_count");
            int totalPages = request.PageSize > 0 ? (int)Math.Ceiling((double)totalCount / request.PageSize) : 0;

            return new TaskListResponseDto
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize,
                TotalPages = totalPages,
                Tasks      = tasks
            };
        }

        // ─── API 2: sp_get_projects_dropdown
        public async Task<TaskProjectDropdownResponseDto> GetProjectDropdownAsync(TaskProjectDropdownRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",  request.EmpId,    DbType.Int32);
            parameters.Add("@usr_typ", request.UserType, DbType.Int32);

            var projects = await connection.QueryAsync<TaskProjectDropdownDto>(
                "PMTool_v2.sp_get_projects_dropdown",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new TaskProjectDropdownResponseDto { Projects = projects };
        }

        // ─── API 3: sp_get_distinct_projects
        public async Task<TaskProjectFilterResponseDto> GetProjectFilterAsync(TaskListRequestDto request)
        {
            if (!DateTime.TryParse(request.StartDate, out var startDate))
                throw new ArgumentException($"Invalid startDate: '{request.StartDate}'. Expected yyyy-MM-dd.");
            if (!DateTime.TryParse(request.EndDate, out var endDate))
                throw new ArgumentException($"Invalid endDate: '{request.EndDate}'. Expected yyyy-MM-dd.");

            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",     request.EmpId,    DbType.Int32);
            parameters.Add("@usr_typ",    request.UserType, DbType.Int32);
            parameters.Add("@start_date", startDate,        DbType.Date);
            parameters.Add("@end_date",   endDate,          DbType.Date);

            var filter = await connection.QueryAsync<TaskProjectFilterDto>(
                "PMTool_v2.sp_get_distinct_projects",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new TaskProjectFilterResponseDto { ProjectFilter = filter };
        }

        // ─── API 4: Sp_DeleteTaskByTaskId
        public async Task<TaskDeleteResponseDto> DeleteTaskAsync(int taskId, TaskDeleteRequestDto body)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId",       taskId,            DbType.Int32);
            parameters.Add("@DeletedBy",    body.DeletedBy,    DbType.Int32);
            parameters.Add("@DeleteReason", body.DeleteReason, DbType.String);

            var result = await connection.QueryFirstOrDefaultAsync<TaskDeleteResponseDto>(
                "PMTool_v2.Sp_DeleteTaskByTaskId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new TaskDeleteResponseDto { Message = "Task not found." };
        }

        // ─── API 5: Sp_BindAllDrpDwnList + SP_GetBuyingCenterEmps
        public async Task<TaskFormDataResponseDto> GetTaskFormDataAsync(TaskFormDataRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var ddlParams = new DynamicParameters();
            ddlParams.Add("@ProjectId", request.ProjectId,  DbType.Int32);
            ddlParams.Add("@taskId",    request.TemplateId, DbType.Int32);

            var multi = await connection.QueryMultipleAsync(
                "Sp_BindAllDrpDwnList",
                ddlParams,
                commandType: CommandType.StoredProcedure
            );

            var clientRow   = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var projectRow  = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var statuses    = (await multi.ReadAsync<TaskStatusDto>()).ToList();
            var templateRow = request.TemplateId > 0
                ? (await multi.ReadAsync<dynamic>()).FirstOrDefault()
                : null;

            IEnumerable<TaskEmployeeDto> employees;
            if (request.BuyingCenterId.HasValue && request.BuyingCenterId > 0)
            {
                var empParams = new DynamicParameters();
                empParams.Add("@Bid", request.BuyingCenterId, DbType.Int32);
                employees = await connection.QueryAsync<TaskEmployeeDto>(
                    "SP_GetBuyingCenterEmps",
                    empParams,
                    commandType: CommandType.StoredProcedure
                );
            }
            else
            {
                employees = await connection.QueryAsync<TaskEmployeeDto>(
                    "SELECT EmpId, Employee_Name FROM Tbl_Employee WHERE ISACTIVE=1 ORDER BY Employee_Name ASC"
                );
            }

            return new TaskFormDataResponseDto
            {
                ClientName   = clientRow?.Client_Title   ?? string.Empty,
                ClientId     = clientRow?.Client_ID      ?? 0,
                ProjectName  = projectRow?.project_Title ?? string.Empty,
                ProjectId    = projectRow?.project_ID    ?? 0,
                Statuses     = statuses,
                Employees    = employees,
                TemplateInfo = templateRow != null ? new TaskTemplateInfoDto
                {
                    TaskName       = templateRow.TaskName       ?? string.Empty,
                    ActualHours    = templateRow.ActualHours    ?? string.Empty,
                    Responsibility = templateRow.Responsibility ?? string.Empty
                } : null
            };
        }

        // ─── API 6: SP_GetTaskDetailsById
        public async Task<EditTaskResponseDto> GetTaskByIdAsync(int taskId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@taskid", taskId.ToString(), DbType.String);

            var multi = await connection.QueryMultipleAsync(
                "SP_GetTaskDetailsById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Table[0]: task details row
            var taskRow = (await multi.ReadAsync<dynamic>()).FirstOrDefault();

            if (taskRow == null)
                return new EditTaskResponseDto();


            // =====================================================
            // GET PROJECT CODE
            // =====================================================

         


            // Table[1]: pivot - working hours per emp per date
            var hoursRows = (await multi.ReadAsync<dynamic>()).ToList();
            // Table[2]: pivot - start times per emp per date
            var startRows  = (await multi.ReadAsync<dynamic>()).ToList();
            // Table[3]: pivot - end times per emp per date
            var endRows    = (await multi.ReadAsync<dynamic>()).ToList();
            // Table[4]: comma-separated dates string
            var datesRow   = (await multi.ReadAsync<dynamic>()).FirstOrDefault();

            string datesStr = datesRow?.Dates ?? string.Empty;
            var dates = datesStr.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()).ToList();

            // Reconstruct allocations from pivot tables
            var employeeAllocations = new List<EmployeeAllocationDto>();
            foreach (var hRow in hoursRows)
            {
                var hDict = (IDictionary<string, object>)hRow;
                int empId = hDict.ContainsKey("EmpId") && hDict["EmpId"] != null ? Convert.ToInt32(hDict["EmpId"]) : 0;
                if (empId == 0) continue;

                var sRow = startRows.Cast<IDictionary<string, object>>().FirstOrDefault(r => r.ContainsKey("EmpId") && Convert.ToInt32(r["EmpId"]) == empId);
                var eRow = endRows.Cast<IDictionary<string, object>>().FirstOrDefault(r => r.ContainsKey("EmpId") && Convert.ToInt32(r["EmpId"]) == empId);

                var dailyAllocations = new List<DailyAllocationDto>();
                foreach (var date in dates)
                {
                    if (!hDict.ContainsKey(date) || hDict[date] == null) continue;

                    float hours = hDict[date] != null ? Convert.ToSingle(hDict[date]) : 0f;
                    string startTime = sRow != null && sRow.ContainsKey(date) && sRow[date] != null ? sRow[date].ToString()! : string.Empty;
                    string endTime   = eRow != null && eRow.ContainsKey(date) && eRow[date] != null ? eRow[date].ToString()! : string.Empty;

                    // parse date from MM/dd/yyyy (SP uses CONVERT varchar 101)
                    string isoDate = DateTime.TryParse(date, out var parsedDate) ? parsedDate.ToString("yyyy-MM-dd") : date;

                    dailyAllocations.Add(new DailyAllocationDto
                    {
                        AllocationDate = isoDate,
                        Hours          = hours,
                        StartTime      = startTime,
                        EndTime        = endTime
                    });
                }

                employeeAllocations.Add(new EmployeeAllocationDto
                {
                    EmployeeId       = empId,
                    DailyAllocations = dailyAllocations
                });
            }

            int tempId = taskRow.tempId != null ? (int)taskRow.tempId : 0;
            TaskTemplateInfoDto? templateInfo = null;

            if (tempId > 0)
            {
                var tplParams = new DynamicParameters();
                tplParams.Add("@ProjectId", 0,      DbType.Int32);
                tplParams.Add("@taskId",    tempId, DbType.Int32);

                var tplMulti = await connection.QueryMultipleAsync(
                    "Sp_BindAllDrpDwnList",
                    tplParams,
                    commandType: CommandType.StoredProcedure
                );
                await tplMulti.ReadAsync<dynamic>();
                await tplMulti.ReadAsync<dynamic>();
                await tplMulti.ReadAsync<dynamic>();
                var tplRow = (await tplMulti.ReadAsync<dynamic>()).FirstOrDefault();

                if (tplRow != null)
                    templateInfo = new TaskTemplateInfoDto
                    {
                        TaskName       = tplRow.TaskName       ?? string.Empty,
                        ActualHours    = tplRow.ActualHours    ?? string.Empty,
                        Responsibility = tplRow.Responsibility ?? string.Empty
                    };
            }

            return new EditTaskResponseDto
            {
                TaskId = taskRow.Task_Id != null
        ? Convert.ToInt32(taskRow.Task_Id)
        : 0,

                TaskTitle = taskRow.Task_Title
        ?? string.Empty,

                EmpIds = taskRow.Employee_Name
        ?? string.Empty,

                StartDate = taskRow.startdate?.ToString()
        ?? string.Empty,

                EndDate = taskRow.enddate?.ToString()
        ?? string.Empty,

                Status = taskRow.status != null
        ? Convert.ToInt32(taskRow.status)
        : 0,

                TaskPriority = taskRow.tskpriority
        ?? string.Empty,

                ProjectId = taskRow.ProjectId != null
    ? Convert.ToInt32(taskRow.ProjectId)
    : 0,

                ProjectCode = taskRow.ProjectCode != null
    ? taskRow.ProjectCode.ToString()
    : string.Empty,

                ClientId = taskRow.ClientID != null
    ? Convert.ToInt32(taskRow.ClientID)
    : 0,

                TemplateId = tempId,

                EmployeeAllocations = employeeAllocations,

                TemplateInfo = templateInfo
            };
        }

        // ─── API 7: Sp_Create_task + Sp_InsertflexiblehrsByEmp
        public async Task<CreateTaskResponseDto> CreateTaskAsync(CreateTaskRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            string empIds = string.Join(",", request.EmployeeAllocations.Select(a => a.EmployeeId).Distinct());

            var parameters = new DynamicParameters();
            parameters.Add("@taskTitle",    request.TaskTitle,           DbType.String);
            parameters.Add("@emp_name",     empIds,                      DbType.String);
            parameters.Add("@startDate",    request.StartDate,           DbType.String);
            parameters.Add("@endDate",      request.EndDate,             DbType.String);
            parameters.Add("@status",       request.StatusId.ToString(), DbType.String);
            parameters.Add("@taskpriority", request.Priority,            DbType.String);
            parameters.Add("@projectId",    request.ProjectId,           DbType.Int32);
            parameters.Add("@created_by",   request.CreatedBy,           DbType.Int32);
            parameters.Add("@clientId",     request.ClientId,            DbType.Int32);
            parameters.Add("@tempId",       request.TemplateId.ToString(), DbType.String);
            parameters.Add("@RetTaskId",    dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "Sp_Create_task",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            string retVal   = parameters.Get<string>("@RetTaskId");
            int    retTaskId = int.TryParse(retVal, out var parsed) ? parsed : 0;

            if (retTaskId > 0)
                await _insertAllocationsAsync(connection, retTaskId, request.StatusId, request.EmployeeAllocations);

            return new CreateTaskResponseDto
            {
                TaskId  = retTaskId,
                Message = retTaskId > 0 ? "Task created successfully." : "Task already exists."
            };
        }

        // ─── API 8: SP_insertTaskDetails + Sp_InsertflexiblehrsByEmp
        public async Task<CreateTaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            string empIds = string.Join(",", request.EmployeeAllocations.Select(a => a.EmployeeId).Distinct());

            var original = request.OriginalEmpIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var current  = request.EmployeeAllocations.Select(a => a.EmployeeId.ToString()).Distinct();
            var removed  = original.Except(current);

            foreach (var empId in removed)
                await connection.ExecuteAsync(
                    $"DELETE FROM Tbl_EmpTaskDetails WHERE Emp_ID={empId} AND Task_ID={taskId}"
                );

            var parameters = new DynamicParameters();
            parameters.Add("@taskTitle",    request.TaskTitle,           DbType.String);
            parameters.Add("@empname",      empIds,                      DbType.String);
            parameters.Add("@startdate",    request.StartDate,           DbType.String);
            parameters.Add("@enddate",      request.EndDate,             DbType.String);
            parameters.Add("@status",       request.StatusId.ToString(), DbType.String);
            parameters.Add("@taskpriority", request.Priority,            DbType.String);
            parameters.Add("@projectid",    request.ProjectId,           DbType.Int32);
            parameters.Add("@clientid",     request.ClientId,            DbType.Int32);
            parameters.Add("@taskid",       taskId,                      DbType.Int32);
            parameters.Add("@created_by",   request.UpdatedBy,           DbType.Int32);
            parameters.Add("@msg",          dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "SP_insertTaskDetails",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            await _insertAllocationsAsync(connection, taskId, request.StatusId, request.EmployeeAllocations);

            return new CreateTaskResponseDto { TaskId = taskId, Message = "Task updated successfully." };
        }

        // ─── GET tasks by project id: PMTool_v2.sp_get_all_task_details_ProjectId
        public async Task<TaskByProjectResponseDto> GetTasksByProjectIdAsync(TaskByProjectRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@emp_id",      request.EmpId,      DbType.Int32);
            parameters.Add("@usr_typ",     request.UserType,   DbType.Int32);
            parameters.Add("@project_Id",  request.ProjectId,  DbType.Int32);
            parameters.Add("@search_text", request.SearchText, DbType.String);
            parameters.Add("@page_number", request.PageNumber, DbType.Int32);
            parameters.Add("@page_size",   request.PageSize,   DbType.Int32);
            parameters.Add("@total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var tasks = (await connection.QueryAsync<TaskRowDto>(
                "PMTool_v2.sp_get_all_task_details_ProjectId",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            int totalCount = parameters.Get<int>("@total_count");
            int totalPages = request.PageSize > 0 ? (int)Math.Ceiling((double)totalCount / request.PageSize) : 0;

            return new TaskByProjectResponseDto
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize,
                TotalPages = totalPages,
                Tasks      = tasks
            };
        }

        // ─── GET project cost info: GetProjectDetailsById + sp_get_utilization_data
        public async Task<ProjectCostInfoResponseDto> GetProjectCostInfoAsync(int projectId, int buyingCenterId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var costRow = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "GetProjectDetailsById",
                new { projectid = projectId },
                commandType: CommandType.StoredProcedure);

            double totalValue       = costRow?.Total_Value       != null ? (double)costRow.Total_Value       : 0;
            double predictedValue   = costRow?.Predicted_Value   != null ? (double)costRow.Predicted_Value   : 0;
            double outsourcingValue = costRow?.Outsourcing_Value  != null ? (double)costRow.Outsourcing_Value  : 0;

            double utilizationCost = 0;
            if (buyingCenterId > 0)
            {
                var utilParams = new DynamicParameters();
                utilParams.Add("@proj_id", projectId,      DbType.Int32);
                utilParams.Add("@b_id",    buyingCenterId, DbType.Int32);

                var utilRows = await connection.QueryAsync<dynamic>(
                    "sp_get_utilization_data",
                    utilParams,
                    commandType: CommandType.StoredProcedure);

                utilizationCost = utilRows
                    .Where(r => r.Utilization != null)
                    .Sum(r => (double)r.Utilization);
            }

            return new ProjectCostInfoResponseDto
            {
                TotalValue       = totalValue,
                PredictedValue   = predictedValue,
                OutsourcingValue = outsourcingValue,
                UtilizationCost  = utilizationCost
            };
        }

        // ─── shared helper: clear + re-insert allocations per emp per day
        private async System.Threading.Tasks.Task _insertAllocationsAsync(
            IDbConnection connection, int taskId, int status, List<EmployeeAllocationDto> employeeAllocations)
        {
            await connection.ExecuteAsync($"DELETE FROM Tbl_EmpTaskDetails WHERE Task_ID={taskId}");

            float totalHours = 0;
            foreach (var emp in employeeAllocations)
            {
                foreach (var day in emp.DailyAllocations)
                {
                    var p = new DynamicParameters();
                    p.Add("@taskid",        taskId,                      DbType.Int32);
                    p.Add("@empid",         emp.EmployeeId,              DbType.Int32);
                    p.Add("@status",        status,                      DbType.Int32);
                    p.Add("@workhours",     day.Hours,                   DbType.Double);
                    p.Add("@AllocatedDate", DateTime.Parse(day.AllocationDate), DbType.DateTime);
                    p.Add("@AlloStartTime", TimeSpan.Parse(day.StartTime),     DbType.Time);
                    p.Add("@AlloEndTime",   TimeSpan.Parse(day.EndTime),       DbType.Time);
                    p.Add("@msg",           dbType: DbType.String, size: 50,   direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "Sp_InsertflexiblehrsByEmp",
                        p,
                        commandType: CommandType.StoredProcedure
                    );
                    totalHours += day.Hours;
                }
            }

            await connection.ExecuteAsync(
                $"UPDATE Tbl_Task_Details SET Working_hours={totalHours} WHERE Task_Id={taskId}"
            );
        }

        //  SP_Reschedule_Task
        public async Task<RescheduleTaskResponseDto> RescheduleTaskAsync(
            RescheduleTaskRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Task_Id", request.TaskId, DbType.Int32);
            parameters.Add("@Employee_Id", request.EmployeeId, DbType.Int32);
            parameters.Add("@Original_Date", request.OriginalDate, DbType.Date);
            parameters.Add("@Reschedule_Date", request.RescheduleDate, DbType.Date);
            parameters.Add("@Reschedule_Type", request.RescheduleType, DbType.String);
            parameters.Add("@Reason", request.Reason, DbType.String);
            parameters.Add("@Created_By", request.CreatedBy, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<RescheduleTaskResponseDto>(
                "PMTool_v2.SP_Reschedule_Task",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new RescheduleTaskResponseDto
            {
                Success = 0,
                Message = "Unable to reschedule task."
            };
        }

        public async Task<AddTaskCommentResponseDto> AddTaskCommentAsync(
    AddTaskCommentRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Task_Id", request.TaskId, DbType.Int32);
            parameters.Add("@Employee_Id", request.EmployeeId, DbType.Int32);
            parameters.Add("@Comment_Type", request.CommentType, DbType.String);
            parameters.Add("@Comment_Text", request.CommentText, DbType.String);
            parameters.Add("@Planned_Hours", request.PlannedHours, DbType.Decimal);
            parameters.Add("@Actual_Hours", request.ActualHours, DbType.Decimal);
            parameters.Add("@Created_By", request.CreatedBy, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<AddTaskCommentResponseDto>(
                "PMTool_v2.SP_Add_Task_Comment",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new AddTaskCommentResponseDto
            {
                Success = 0,
                Message = "Unable to add task comment."
            };
        }

        public async Task<List<GetTaskCommentsResponseDto>> GetTaskCommentsAsync(
    int taskId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Task_Id", taskId, DbType.Int32);

            var result = await connection.QueryAsync<GetTaskCommentsResponseDto>(
                "PMTool_v2.SP_Get_Task_Comments",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<MarkTaskCommentReadResponseDto> MarkTaskCommentReadAsync(
    int commentId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Comment_Id", commentId, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<MarkTaskCommentReadResponseDto>(
                "PMTool_v2.SP_Mark_Task_Comment_Read",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new MarkTaskCommentReadResponseDto
            {
                Success = 0,
                Message = "Unable to mark task comment as read.",
                Comment_Id = commentId
            };
        }

        public async Task<List<GetTaskNotificationsResponseDto>> GetTaskNotificationsAsync(
    int recipientId,
    bool unreadOnly)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Recipient_Id", recipientId, DbType.Int32);
            parameters.Add("@UnreadOnly", unreadOnly, DbType.Boolean);

            var result = await connection.QueryAsync<GetTaskNotificationsResponseDto>(
                "PMTool_v2.SP_Get_Task_Notifications",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<MarkTaskNotificationReadResponseDto> MarkTaskNotificationReadAsync(
    int notificationId)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@Notification_Id", notificationId, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<MarkTaskNotificationReadResponseDto>(
                "PMTool_v2.SP_Mark_Task_Notification_Read",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new MarkTaskNotificationReadResponseDto
            {
                Success = 0,
                Message = "Unable to mark notification as read.",
                Notification_Id = notificationId
            };
        }

        public async Task<TaskTemplateListResponseDto> GetTaskTemplateListAsync()
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetTaskTemplateList",
                commandType: CommandType.StoredProcedure
            );

            var tasks = await multi.ReadAsync<TaskTemplateListDto>();

            var templates = await multi.ReadAsync<TaskTemplateDto>();

            var projects = await multi.ReadAsync<TaskTemplateProjectDto>();

            return new TaskTemplateListResponseDto
            {
                Tasks = tasks.ToList(),
                Templates = templates.ToList(),
                Projects = projects.ToList()
            };
        }

        public async Task<CreateTaskTemplateResponseDto> CreateTaskTemplateAsync(
    CreateTaskTemplateRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add(
                "@templateName",
                request.TemplateName,
                DbType.String
            );

            parameters.Add(
                "@TaskDetails",
                request.TaskDetails,
                DbType.String
            );

            parameters.Add(
                "@TaskName",
                request.TaskName,
                DbType.String
            );

            parameters.Add(
                "@Responsibilities",
                request.Responsibilities,
                DbType.String
            );

            parameters.Add(
                "@ActualHours",
                request.ActualHours,
                DbType.String
            );

            parameters.Add(
                "@createBy",
                request.CreateBy,
                DbType.Int32
            );

            await connection.ExecuteAsync(
                "SP_InsertTaskTemplate",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120
            );

            return new CreateTaskTemplateResponseDto
            {
                Success = true,
                Message = "Task template created successfully."
            };
        }
    }
}
