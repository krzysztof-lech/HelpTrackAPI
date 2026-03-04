using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;

namespace HelpTrackAPI.Services.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetTicketsAsync(int currentUserId, Role currentRole, bool onlyMine = false); 
        Task<TicketDto?> GetTicketByIdAsync(int id, int currentUserId, Role currentRole); 
        Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, int currentUserId);
        Task<TicketDto> UpdateTicketAsync(int id, UpdateTicketDto updateDto, int currentUserId, Role currentRole);
        Task DeleteTicketAsync(int id, int currentUserId, Role currentRole);
        Task<TicketDto> AssignTicketAsync(int id, AssignTicketDto dto, int currentUserId, Role currentRole);
        Task<TicketDto> UpdateTicketStatusAsync(int id, UpdateTicketStatusDto dto, int currentUserId, Role currentRole);
        Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId, int currentUserId, Role currentRole);

    }
}
