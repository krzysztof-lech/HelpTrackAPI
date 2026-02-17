using System.ComponentModel.DataAnnotations;

namespace HelpTrackAPI.Models.Dtos
{
    public class CreateTicketDto
    {
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }
    }
}
