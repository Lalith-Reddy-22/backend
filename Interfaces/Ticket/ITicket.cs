using Medtrix_PmTool_V2_Api.DTOs.Ticket;

namespace Medtrix_PmTool_V2_Api.Interfaces.Ticket
{
    public interface ITicket
    {
        Task<GetTicketListResponseDto> GetTicketListAsync(GetTicketListRequestDto request);
        Task<TicketFormDataResponseDto> GetFormDataAsync();
        Task<CreateTicketResponseDto> CreateTicketAsync(CreateTicketRequestDto request);
        Task<UpdateTicketStatusResponseDto> UpdateTicketStatusAsync(UpdateTicketStatusRequestDto request);
    }
}
