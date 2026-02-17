using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;

namespace HelpTrackAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync(int currentUserId, Role currentRole); 
        Task<UserDto?> GetUserByIdAsync(int id, int currentUserId, Role currentRole); 
        Task<UserDto> CreateUserAsync(CreateUserDto dto); 
        Task UpdateUserAsync(UpdateUserDto dto, int currentUserId, Role currentRole); 
        Task DeleteUserAsync(int id, int currentUserId, Role currentRole);
    }
}
