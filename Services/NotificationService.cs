using HelpTrackAPI.Data;
using HelpTrackAPI.Models;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpTrackAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HelpTrackContext _context;
        private const string WelcomeMessage = "Witaj w HelpTrack. Tutaj będziesz widzieć swoje powiadomienia.";

        public NotificationService(HelpTrackContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            if (!notifications.Any())
            {
                var welcome = new Notification
                {
                    UserId = userId,
                    Message = WelcomeMessage,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(welcome);
                await _context.SaveChangesAsync();

                notifications.Add(welcome);
            }

            return notifications;
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException("Cannot mark other users' notifications.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationForTicketCreatedAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return;

            var supportAgents = await _context.Users
                .Where(u => u.Role == Role.SupportAgent)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var agentId in supportAgents)
            {
                var message = $"Nowe zgłoszenie: '{ticket.Title}'";
                var notification = new Notification
                {
                    UserId = agentId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationForTicketAssignedAsync(int ticketId, int assignedToUserId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return;

            var message = $"Zgłoszenie '{ticket.Title}' zostało Ci przypisane";

            var notification = new Notification
            {
                UserId = assignedToUserId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationForNewMessageAsync(int ticketId, int authorId, string messagePreview)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return;

            var recipientIds = new HashSet<int>();

            if (ticket.UserId != authorId)
                recipientIds.Add(ticket.UserId);

            if (ticket.AssignedToUserId.HasValue && ticket.AssignedToUserId != authorId)
                recipientIds.Add(ticket.AssignedToUserId.Value);

            foreach (var recipientId in recipientIds)
            {
                var message = $"Nowa wiadomość w zgłoszeniu '{ticket.Title}': \"{messagePreview}\"";

                var notification = new Notification
                {
                    UserId = recipientId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationForStatusChangedAsync(int ticketId, int changedByUserId, string newStatus)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return;

            var recipientIds = new HashSet<int>();

            if (ticket.UserId != changedByUserId)
                recipientIds.Add(ticket.UserId);

            if (ticket.AssignedToUserId.HasValue && ticket.AssignedToUserId != changedByUserId)
                recipientIds.Add(ticket.AssignedToUserId.Value);

            foreach (var recipientId in recipientIds)
            {
                var message = $"Status zgłoszenia '{ticket.Title}' zmienił się na: {newStatus}";

                var notification = new Notification
                {
                    UserId = recipientId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }
    }
}