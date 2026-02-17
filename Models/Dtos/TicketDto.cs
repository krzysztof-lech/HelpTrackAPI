namespace HelpTrackAPI.Models.Dtos
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public UserDto User { get; set; }
        public int? AssignedToUserId { get; set; }
        public UserDto AssignedToUser { get; set; }
        public bool HasUnreadMessages { get; set; }
    }
}
