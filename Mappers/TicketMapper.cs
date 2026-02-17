using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Models;
using System.Runtime.CompilerServices;

namespace HelpTrackAPI.Mappers
{
    public static class TicketMapper
    {
        public static TicketDto ToDto (this Ticket t)
        {
            return new TicketDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId,
                AssignedToUserId = t.AssignedToUserId,
                HasUnreadMessages = t.HasUnreadMessages,
                User = t.User?.ToDto(),
                AssignedToUser = t.AssignedToUser?.ToDto()
            };
        }

    }
}
