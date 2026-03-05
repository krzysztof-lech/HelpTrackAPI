using HelpTrackAPI.Data;
using HelpTrackAPI.Mappers;
using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpTrackAPI.Services
{
    public class TicketMessageService : ITicketMessageService
    {
        private readonly HelpTrackContext _context; 
        public TicketMessageService(HelpTrackContext context) 
        { 
            _context = context; 
        }

        private bool UserHasAccessToTicket(Ticket ticket, int userId, string role) 
        { 
            if (role == "Admin") 
                return true; 

            if (ticket.UserId == userId) 
                return true;
            
            if (ticket.AssignedToUserId == userId) 
                return true;
            
            return false; 
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int ticketId, int userId, string role) 
        { 
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Messages)
                     .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == ticketId); 
            
            if (ticket == null) 
                throw new KeyNotFoundException("Ticket not found"); 
            
            if (!UserHasAccessToTicket(ticket, userId, role)) 
                throw new UnauthorizedAccessException(); 

            return ticket.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => m.ToDto())
                .ToList(); 
        }

        public async Task<ChatMessageDto> AddMessageAsync(CreateChatMessageDto dto, int userId, string role) 
        { 
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == dto.TicketId); 
            
            if (ticket == null) 
                throw new KeyNotFoundException("Ticket not found"); 
            
            if (!UserHasAccessToTicket(ticket, userId, role)) 
                throw new UnauthorizedAccessException(); 
            
            var message = new TicketMessage 
            { 
                TicketId = dto.TicketId, 
                AuthorId = userId, 
                Content = dto.Message, 
                CreatedAt = DateTime.UtcNow, 
                IsRead = false, 
                IsFromSupport = role == "SupportAgent" 
            };

            ticket.UpdatedAt = DateTime.UtcNow;

            _context.TicketMessages.Add(message); 
            await _context.SaveChangesAsync(); 
            
            await _context.Entry(message).Reference(m => m.Author).LoadAsync(); 
            
            return message.ToDto(); 
        }

        public async Task<int> MarkAsReadAsync(int ticketId, int userId, string role) 
        { 
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId); 
            
            if (ticket == null) 
                throw new KeyNotFoundException("Ticket not found"); 
            
            if (!UserHasAccessToTicket(ticket, userId, role)) 
                throw new UnauthorizedAccessException(); 
            
            var messages = await _context.TicketMessages
                .Where(m => m.TicketId == ticketId && m.AuthorId != userId && !m.IsRead)
                .ToListAsync(); 
            
            foreach (var msg in messages) 
                msg.IsRead = true; 
            
            await _context.SaveChangesAsync(); 
            
            return messages.Count; 
        }
    }
}
