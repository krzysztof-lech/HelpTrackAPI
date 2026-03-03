namespace HelpTrackAPI.Models.Dtos
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsFromSupport { get; set; }
        public bool IsRead { get; set; }

    }
}
