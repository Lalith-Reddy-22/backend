namespace Medtrix_PmTool_V2_Api.DTOs.Employee
{
    public class HashPasswordMigrationResponseDto
    {
        public bool Success { get; set; }
        public int TotalEmployees { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}