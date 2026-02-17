using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;

namespace HelpTrackAPI.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User u)
        {
            return new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                Role = u.Role
            };
        }
    }
}
