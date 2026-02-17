using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpTrackAPI.Models
{
    public enum TicketStatus
    {
        New,
        InProgress,
        Closed
    }

    [Table("ticket")]
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title can be at most 100 characters long")]
        public required string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description can be at most 500 characters long")]
        public string? Description { get; set; } 

        [Required]
        [EnumDataType(typeof(TicketStatus))]
        public TicketStatus Status { get; set; } = TicketStatus.New;

        [Required(ErrorMessage = "Ticket creator is required")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int? AssignedToUserId { get; set; }

        [ForeignKey("AssignedToUserId")]
        public User? AssignedToUser { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public bool HasUnreadMessages { get; set; }
    }
}
