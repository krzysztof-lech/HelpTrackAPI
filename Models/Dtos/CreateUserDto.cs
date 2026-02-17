using System.ComponentModel.DataAnnotations;

namespace HelpTrackAPI.Models.Dtos
{
    public class CreateUserDto
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        public Role Role { get; set; }
    }
}
