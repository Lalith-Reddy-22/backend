using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Ticket;
using Medtrix_PmTool_V2_Api.Interfaces.Ticket;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Ticket
{
    public class TicketRepository : ITicket
    {
        private readonly DbContext.DbContext _db;

        public TicketRepository(DbContext.DbContext db)
        {
            _db = db;
        }

        public async Task<GetTicketListResponseDto> GetTicketListAsync(GetTicketListRequestDto request)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@EmpId",          request.EmpId,                                          DbType.Int32);
            p.Add("@Emp_type",       request.EmpType,                                        DbType.Int32);
            p.Add("@start_date",     string.IsNullOrWhiteSpace(request.StartDate)   ? null : request.StartDate,   DbType.String);
            p.Add("@end_date",       string.IsNullOrWhiteSpace(request.EndDate)     ? null : request.EndDate,     DbType.String);
            p.Add("@search_text",    string.IsNullOrWhiteSpace(request.SearchText)  ? null : request.SearchText,  DbType.String);
            p.Add("@status_id",      request.StatusId,                                       DbType.Int32);
            p.Add("@severity_id",    request.SeverityId,                                     DbType.Int32);
            p.Add("@issue_type_id",  request.IssueTypeId,                                    DbType.Int32);
            p.Add("@page_number",    request.PageNumber,                                     DbType.Int32);
            p.Add("@page_size",      request.PageSize,                                       DbType.Int32);
            p.Add("@total_count",    dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rows = (await conn.QueryAsync<TicketListItemDto>(
                "PMTool_v2.sp_get_all_ticketing_issues",
                p,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalCount = p.Get<int>("@total_count");
            int totalPages = request.PageSize > 0 ? (int)Math.Ceiling((double)totalCount / request.PageSize) : 0;

            return new GetTicketListResponseDto
            {
                Tickets    = rows,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize
            };
        }

        public async Task<TicketFormDataResponseDto> GetFormDataAsync()
        {
            using var conn = _db.CreateConnection();

            using var multi = await conn.QueryMultipleAsync(
                "GetAllTicketingIssueStatus",
                commandType: CommandType.StoredProcedure);

            var statuses   = (await multi.ReadAsync<TicketStatusDto>()).ToList();
            var severities = (await multi.ReadAsync<TicketSeverityDto>()).ToList();
            var issueTypes = (await multi.ReadAsync<TicketIssueTypeDto>()).ToList();
            var employees  = (await multi.ReadAsync<TicketEmployeeDto>()).ToList();

            return new TicketFormDataResponseDto
            {
                Statuses   = statuses,
                Severities = severities,
                IssueTypes = issueTypes,
                Employees  = employees
            };
        }

        public async Task<CreateTicketResponseDto> CreateTicketAsync(CreateTicketRequestDto request)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@Issue_Type",        request.IssueType,        DbType.Int32);
            p.Add("@Sub_Issue_Type",    request.SubIssueType,     DbType.Int32);
            p.Add("@Severity",          request.Severity,         DbType.Int32);
            p.Add("@Issue_Description", request.IssueDescription, DbType.String);
            p.Add("@Emp_Id",            request.EmpId,            DbType.Int32);
            p.Add("@Status",            request.Status,           DbType.Int32);
            p.Add("@Ret_Issue_Id",      dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("Sp_Create_Issue", p, commandType: CommandType.StoredProcedure);

            int issueId = p.Get<int>("@Ret_Issue_Id");

            return new CreateTicketResponseDto
            {
                IssueId = issueId,
                Message = issueId > 0 ? "Ticket created successfully." : "Failed to create ticket."
            };
        }

        public async Task<UpdateTicketStatusResponseDto> UpdateTicketStatusAsync(UpdateTicketStatusRequestDto request)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@Issue_Id",   request.IssueId,   DbType.Int32);
            p.Add("@Status",     request.Status,    DbType.Int32);
            p.Add("@Comments",   request.Comments,  DbType.String);
            p.Add("@Updated_By", request.UpdatedBy, DbType.Int32);
            p.Add("@Message",    dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

            var row = await conn.QueryFirstOrDefaultAsync<UpdateTicketStatusResponseDto>(
                "PMTool_v2.sp_update_issue_status",
                p,
                commandType: CommandType.StoredProcedure);

            string message = p.Get<string>("@Message");

            if (row != null)
            {
                row.Message = message;
                return row;
            }

            return new UpdateTicketStatusResponseDto { Message = message ?? "Issue not found." };
        }
    }
}
