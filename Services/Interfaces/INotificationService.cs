using HelpTrackAPI.Models;

namespace HelpTrackAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task CreateNotificationAsync(int userId, string message);
        Task CreateNotificationForTicketCreatedAsync(int ticketId);
        Task CreateNotificationForTicketAssignedAsync(int ticketId, int assignedToUserId);
        Task CreateNotificationForNewMessageAsync(int ticketId, int authorId, string messagePreview);
        Task CreateNotificationForStatusChangedAsync(int ticketId, int changedByUserId, string newStatus);
    }
}