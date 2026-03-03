using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;

namespace HelpTrackAPI.Mappers
{
    public static class ChatMessageMapper
    {
        public static ChatMessageDto ToDto(this TicketMessage msg) 
        { 
            return new ChatMessageDto 
            { 
                Id = msg.Id, 
                Message = msg.Content, 
                SenderId = msg.AuthorId, 
                SenderName = $"{msg.Author?.FirstName} {msg.Author?.LastName}", 
                SentAt = msg.CreatedAt, 
                IsFromSupport = msg.IsFromSupport, 
                IsRead = msg.IsRead 
            };
        }
    }
}
