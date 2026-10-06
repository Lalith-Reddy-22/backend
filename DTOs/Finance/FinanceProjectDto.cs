namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    // ─── Lookups ──────────────────────────────────────────────────────────────
    public class LookupClientDto
    {
        public int Client_ID { get; set; }
        public string Client_Title { get; set; } = string.Empty;
    }

    public class LookupEmployeeDto
    {
        public int EmpId { get; set; }
        public string Employee_Name { get; set; } = string.Empty;
    }

    public class LookupBuyingCenterDto
    {
        public int BId { get; set; }
        public string BName { get; set; } = string.Empty;
    }

    public class LookupFinancialYearDto
    {
        public int Id { get; set; }
        public string Financial_Year { get; set; } = string.Empty;
    }

    public class LookupProjectStatusDto
    {
        public int Id { get; set; }
        public string Status_Name { get; set; } = string.Empty;
    }

    // ─── All Lookups in one call ──────────────────────────────────────────────
    public class ProjectLookupsResponseDto
    {
        public IEnumerable<LookupClientDto> Clients { get; set; } = Enumerable.Empty<LookupClientDto>();
        public IEnumerable<LookupEmployeeDto> ProjectManagers { get; set; } = Enumerable.Empty<LookupEmployeeDto>();
        public IEnumerable<LookupBuyingCenterDto> BuyingCenters { get; set; } = Enumerable.Empty<LookupBuyingCenterDto>();
        public IEnumerable<LookupFinancialYearDto> FinancialYears { get; set; } = Enumerable.Empty<LookupFinancialYearDto>();
        public IEnumerable<LookupProjectStatusDto> ProjectStatuses { get; set; } = Enumerable.Empty<LookupProjectStatusDto>();
    }
    // ─── Get Project By Id ────────────────────────────────────────────────────
    public class FinanceProjectDetailDto
    {
        public int project_ID { get; set; }
        public int client_Id { get; set; }
        public string project_Title { get; set; } = string.Empty;
        public string? project_desc { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string? project_manager { get; set; }  // comma-separated IDs e.g. "60,113,128"
        public string projectCode { get; set; } = string.Empty;
        public int B_Id { get; set; }
        public double Predicted_Value { get; set; }
        public double Outsourcing_Value { get; set; }
        public double Total_Value { get; set; }
        public double Vendor_Value { get; set; }
        public int? Created_By { get; set; }
        public int Financial_Year_Id { get; set; }
        public string Region { get; set; } = string.Empty;
        public int Project_Status { get; set; }
        public int Billing_type { get; set; }
        public string Sub_Client_Name { get; set; } = string.Empty;
    }

    // ─── Save Project Request ─────────────────────────────────────────────────
    public class SaveProjectRequestDto
    {
        public string ProjectCode { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string ProjectManagerIds { get; set; } = string.Empty;  // e.g. "4,7"
        public int BuyingCenterId { get; set; }
        public int FinancialYearId { get; set; }
        public double PredictedValue { get; set; }
        public double OutsourcingValue { get; set; }
        public double TotalValue { get; set; }
        public double VendorValue { get; set; }
        public int ProjectStatus { get; set; }
        public int BillingType { get; set; }
        public string Region { get; set; } = string.Empty;
        public string SubClientName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreatedBy { get; set; }
    }

    // ─── Save Project Response ────────────────────────────────────────────────
    public class SaveProjectResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int ProjectId { get; set; }
    }

    // ─── Internal: SP result row ──────────────────────────────────────────────
    public class SaveProjectResultRowDto
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
        public string Region { get; set; } = string.Empty;
        public double TotalValue { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
