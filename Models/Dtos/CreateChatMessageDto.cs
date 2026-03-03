namespace HelpTrackAPI.Models.Dtos
{
    public class CreateChatMessageDto
    {
        public int TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
