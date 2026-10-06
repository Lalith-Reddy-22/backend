using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Medtrix_PmTool_V2_Api.Models.RateCard;

namespace Medtrix_PmTool_V2_Api.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class RateCardController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RateCardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/employees/123/rate-card
        [HttpGet("{empId}/rate-card")]
        public async Task<IActionResult> GetRateCard(int empId)
        {
            if (empId <= 0)
            {
                return BadRequest("Invalid employee ID.");
            }

            var result = new List<RateCardItem>();

            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection connection =
                new SqlConnection(connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "SP_GetBuyCenter_By_Empid",
                    connection
                );

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EmpId", empId);

            await connection.OpenAsync();

            using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new RateCardItem
                {
                    BuyingCenterId = Convert.ToInt32(
                        reader["BID"]
                    ),

                    BuyingCenterName =
                        reader["BNAME"]?.ToString(),

                    EmployeeHours =
                        Convert.ToDouble(
                            reader["EMP_VALUE"]
                        )
                });
            }

            return Ok(new
            {
                employeeId = empId,
                buyingCenters = result
            });
        }


        // POST: api/employees/123/rate-card
        [HttpPost("{empId}/rate-card")]
        public async Task<IActionResult> UpdateRateCard(
            int empId,
            [FromBody] UpdateRateCardRequest request)
        {
            if (empId <= 0)
            {
                return BadRequest("Invalid employee ID.");
            }

            if (request == null ||
                request.BuyingCenters == null ||
                request.BuyingCenters.Count == 0)
            {
                return BadRequest(
                    "Buying center data is required."
                );
            }

            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            using SqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                foreach (var item in request.BuyingCenters)
                {
                    using SqlCommand command =
                        new SqlCommand(
                            "SP_InsertRateCardBy_EmpID",
                            connection,
                            transaction
                        );

                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.Parameters.Add(
                        "@B_Id",
                        SqlDbType.Int
                    ).Value = item.BuyingCenterId;

                    command.Parameters.Add(
                        "@Emp_Id",
                        SqlDbType.Int
                    ).Value = empId;

                    command.Parameters.Add(
                        "@Emp_Value",
                        SqlDbType.Float
                    ).Value = item.EmployeeHours;

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = "Rate card updated successfully."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Failed to update rate card.",
                        error = ex.Message
                    }
                );
            }
        }
    }
}