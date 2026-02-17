using HelpTrackAPI.Data;
using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Mappers;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HelpTrackAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly HelpTrackContext _context; 
        public TicketService(HelpTrackContext context) 
        { 
            _context = context; 
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsAsync(int currentUserId, Role currentRole) 
        {
            IQueryable<Ticket> query = _context.Tickets
                .Include(t => t.User)
                .Include(t => t.AssignedToUser); 
            
            if (currentRole == Role.Employee) 
                query = query.Where(t => t.UserId == currentUserId); 
            
            var tickets = await query.ToListAsync(); 
            
            return tickets.Select(t => t.ToDto());
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id, int currentUserId, Role currentRole) 
        { 
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id); 
            
            if (ticket == null) 
                return null; 
            
            if (currentRole == Role.Employee && ticket.UserId != currentUserId) 
                return null; 
            
            return ticket.ToDto(); 
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, int currentUserId) 
        { 
            var ticket = new Ticket
            {   Title = dto.Title, 
                Description = dto.Description, 
                UserId = currentUserId, 
                Status = TicketStatus.New, 
                CreatedAt = DateTime.UtcNow, 
            };

            _context.Tickets.Add(ticket); 
            await _context.SaveChangesAsync(); 

            await _context.Entry(ticket).Reference(t => t.User).LoadAsync();
            await _context.Entry(ticket).Reference(t => t.AssignedToUser).LoadAsync();

            return ticket.ToDto(); 
        }

        public async Task UpdateTicketAsync(int id, UpdateTicketDto updateDto, int currentUserId, Role currentRole) 
        { 
            var existing = await _context.Tickets.FindAsync(id);

            if (existing == null) 
                throw new KeyNotFoundException("Ticket not found"); 
            
            if (currentRole == Role.Employee && existing.UserId != currentUserId) 
                throw new UnauthorizedAccessException(); 
            
            existing.Title = updateDto.Title; 
            existing.Description = updateDto.Description; 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteTicketAsync(int id, int currentUserId, Role currentRole) 
        { 
            var ticket = await _context.Tickets.FindAsync(id); 
            if (ticket == null) 
                throw new KeyNotFoundException($"Ticket with ID {id} does not exist.");
            
            if (currentRole != Role.Admin) 
                throw new UnauthorizedAccessException(); 

            _context.Tickets.Remove(ticket); 
            await _context.SaveChangesAsync(); 
        }

        public async Task<TicketDto> AssignTicketAsync(int id, AssignTicketDto dto, int currentUserId, Role currentRole)
        {
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                throw new Exception("Ticket not found");

            if (currentRole == Role.Employee)
                throw new UnauthorizedAccessException();

            ticket.AssignedToUserId = dto.AssignedToUserId;
            await _context.SaveChangesAsync();

            return ticket.ToDto();
        }


        public async Task<TicketDto> UpdateTicketStatusAsync(int id, UpdateTicketStatusDto dto, int currentUserId, Role currentRole) 
        { 
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) 
                throw new KeyNotFoundException("Ticket not found");

            if (currentRole == Role.Employee)
                throw new UnauthorizedAccessException("Employees cannot change ticket statuses.");

            if (currentRole == Role.SupportAgent && ticket.AssignedToUserId != currentUserId)
                throw new UnauthorizedAccessException("You are not assigned to this ticket.");

            ticket.Status = dto.Status; 
            await _context.SaveChangesAsync();

            return ticket.ToDto();
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId, int currentUserId, Role currentRole)
        {
            if (currentRole == Role.Employee && currentUserId != userId)
                throw new UnauthorizedAccessException();

            var tickets = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.AssignedToUser)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                ticket.HasUnreadMessages = await _context.TicketMessages
                    .AnyAsync(m => m.TicketId == ticket.Id
                                && !m.IsRead
                                && m.AuthorId != userId);
            }

            return tickets.Select(t => t.ToDto());
        }

    }
}
