using HelpTrackAPI.Models.Dtos;

namespace HelpTrackAPI.Services.Interfaces
{
    public interface ITicketMessageService
    {
        Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int ticketId, int userId, string role); 
        Task<ChatMessageDto> AddMessageAsync(CreateChatMessageDto dto, int userId, string role); 
        Task<int> MarkAsReadAsync(int ticketId, int userId, string role);
    }
}
