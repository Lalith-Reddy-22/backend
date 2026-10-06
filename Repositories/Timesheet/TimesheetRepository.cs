using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Task;
using Medtrix_PmTool_V2_Api.DTOs.Timesheet;
using Medtrix_PmTool_V2_Api.Interfaces.Timesheet;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Timesheet
{
    public class TimesheetRepository : ITimesheet
    {
        private readonly DbContext.DbContext _dbContext;

        public async Task<string> RescheduleTaskAsync(
    RescheduleTaskRequestDto request)
        {
            return "Task rescheduled successfully.";
        }

        public TimesheetRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET /api/timesheet?empId=3&startDate=2024-01-08&endDate=2024-01-12
        // SP: sp_get_log_sheet_data_byEmpid
        // Result set 1: task list (Task_Id, Task_Title, tskpriority, Taskowner)
        // Result set 2: log detail rows (Task_Id, status, Working_hours, dates, Loghours, LogId)
        public async Task<TimesheetResponseDto> GetTimesheetAsync(
    TimesheetRequestDto request)
        {
            try
            {
                using IDbConnection connection = _dbContext.CreateConnection();

                var parameters = new DynamicParameters();

                parameters.Add(
                    "@start_date",
                    request.StartDate,
                    DbType.String
                );

                parameters.Add(
                    "@end_date",
                    request.EndDate,
                    DbType.String
                );

                parameters.Add(
                    "@Emp_id",
                    request.EmpId,
                    DbType.Int32
                );

                using var multi = await connection.QueryMultipleAsync(
                    "sp_get_log_sheet_data_byEmpid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var taskList =
                    (await multi.ReadAsync<RawTaskDto>()).ToList();

                var logDetails = !multi.IsConsumed
                    ? (await multi.ReadAsync<RawLogDetailDto>()).ToList()
                    : new List<RawLogDetailDto>();

                var tasks = taskList.Select(task =>
                {
                    var taskLogs = logDetails
                        .Where(l => l.Task_Id == task.Task_Id)
                        .Select(l => new TimesheetDateEntryDto
                        {
                            Allocated_Date = l.Allocated_Date,
                            Working_hours = l.Working_hours,
                            AllocStartTime = l.AllocStartTime.ToString(@"hh\:mm"),
                            AllocEndTime = l.AllocEndTime.ToString(@"hh\:mm"),
                            LogId = l.LogId,
                            Loghours = l.Loghours,
                            status = l.status
                        })
                        .ToList();

                    return new TimesheetTaskDto
                    {
                        Task_Id = task.Task_Id,
                        Task_Title = task.Task_Title,
                        tskpriority = task.tskpriority,
                        BuyingCenterId = task.BuyingCenterId,
                        Taskowner = task.Taskowner,
                        dates = taskLogs
                    };
                }).ToList();

                return new TimesheetResponseDto
                {
                    tasks = tasks
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("TIMESHEET ERROR:");
                Console.WriteLine(ex.ToString());

                throw;
            }
        }

        // POST /api/timesheet/log
        // SP: Sp_AddTimeLog — insert or update a time log entry
        // BillingType is always 1 (not exposed to caller)
        public async Task<string> AddTimeLogAsync(AddTimeLogRequestDto request)
        {
            using IDbConnection connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TimeLogId",   request.TimeLogId,         DbType.Int32);
            parameters.Add("@Task_Id",     request.TaskId,            DbType.Int32);
            parameters.Add("@Emp_Id",      request.EmpId,             DbType.Int32);
            parameters.Add("@Log_Date",    request.LogDate,           DbType.DateTime);
            parameters.Add("@Log_hours",   request.LogHours,          DbType.Double);
            parameters.Add("@billingType", 1,                         DbType.Int32);   // always 1
            parameters.Add("@Notes",       request.Notes ?? string.Empty, DbType.String);
            parameters.Add("@msg",         dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "Sp_AddTimeLog",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<string>("@msg") ?? string.Empty;
        }
    }
}
