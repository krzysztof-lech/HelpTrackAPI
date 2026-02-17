using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpTrackAPI.Models
{
    public enum Role
    {
        Employee,
        SupportAgent,
        Admin
    }

    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name can be at most 50 characters long")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name can be at most 50 characters long")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, ErrorMessage = "Username can be at most 20 characters long")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; } = Role.Employee;
    }
}
