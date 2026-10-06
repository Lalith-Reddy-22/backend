using Dapper;
using Medtrix_PmTool_V2_Api.DbContext;
using Medtrix_PmTool_V2_Api.DTOs.Client;
using Medtrix_PmTool_V2_Api.Interfaces.Client;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Client
{
    public class ClientRepository : IClient
    {
        private readonly DbContext.DbContext _dbContext;

        public ClientRepository(DbContext.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ClientDto>> GetAllClientsAsync()
        {
            using var conn = _dbContext.CreateConnection();

            var clients = await conn.QueryAsync<ClientDto>(
                "SP_GetAllClientList",
                commandType: CommandType.StoredProcedure);

            return clients.ToList();
        }
    }
}