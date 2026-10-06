using Medtrix_PmTool_V2_Api.DTOs.Client;

namespace Medtrix_PmTool_V2_Api.Interfaces.Client
{
    public interface IClient
    {
        Task<List<ClientDto>> GetAllClientsAsync();
    }
}