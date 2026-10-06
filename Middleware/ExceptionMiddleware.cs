using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using System.Text.Json;

namespace Medtrix_PmTool_V2_Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                // Dapper / ADO type mapping error (e.g. wrong DTO property type)
                DataException dataEx       => (HttpStatusCode.InternalServerError, BuildMessage("Data mapping error. Please contact support.", dataEx)),

                // SQL connectivity / query errors
                SqlException sqlEx         => (HttpStatusCode.ServiceUnavailable,  BuildMessage("Database error. Please try again later.", sqlEx)),

                // Bad input from caller
                ArgumentNullException      => (HttpStatusCode.BadRequest,          "A required parameter is missing."),
                ArgumentException argEx    => (HttpStatusCode.BadRequest,          argEx.Message),

                // Catch-all
                _                          => (HttpStatusCode.InternalServerError,  BuildMessage("An unexpected error occurred.", ex))
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message    = message,
                // Only expose detailed error info in Development
                Detail     = _env.IsDevelopment() ? ex.ToString() : null
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(json);
        }

        private static string BuildMessage(string fallback, Exception ex)
        {
            // In case inner exception has a more specific message, surface it cleanly
            return string.IsNullOrWhiteSpace(ex.Message) ? fallback : fallback;
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
    }
}
