namespace Medtrix_PmTool_V2_Api.DTOs.Finance
{
    public class BuyingCenterEmployeeDto
    {
        public int Project_Id { get; set; }

        public int Emp_Id { get; set; }

        public string Employee_Name { get; set; } = string.Empty;

        public decimal Emp_Value { get; set; }

        public string BName { get; set; } = string.Empty;
    }
}