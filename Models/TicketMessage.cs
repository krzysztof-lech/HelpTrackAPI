using System.ComponentModel.DataAnnotations.Schema;

namespace HelpTrackAPI.Models
{
    public class TicketMessage
    {
        
        public int Id { get; set; }
        public int TicketId { get; set; }
        
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User? Author { get; set; }


        public string Content { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsRead { get; set; } = false;

        public bool IsFromSupport { get; set; }
    }
}
