using HelpTrackAPI.Data;
using HelpTrackAPI.Mappers;
using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpTrackAPI.Services
{
    public class UserService : IUserService
    {
        private readonly HelpTrackContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(HelpTrackContext context)
        {

            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(int currentUserId, Role currentRole)
        {
            IQueryable<User> query = _context.Users;

            if (currentRole == Role.Employee)
            {
                query = query.Where(u => u.Id == currentUserId);
            }

            var users = await query.ToListAsync();

            return users.Select(u => u.ToDto());
        }

        public async Task<UserDto?> GetUserByIdAsync(int id, int currentUserId, Role currentRole)
        {
            var user = await _context.Users.FindAsync(id); 

            if (user == null) 
                return null; 

            if (currentRole == Role.Employee && user.Id != currentUserId) 
                return null; 

            return user.ToDto();
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto) 
        { 
            var user = new User 
            { 
                FirstName = dto.FirstName, 
                LastName = dto.LastName, 
                Username = dto.Username, 
                Role = dto.Role, 
                Password = "" 
            };

            user.Password = _passwordHasher.HashPassword(user, dto.Password); 

            _context.Users.Add(user); 
            await _context.SaveChangesAsync(); 

            return user.ToDto(); 
        }

        public async Task UpdateUserAsync(UpdateUserDto dto, int currentUserId, Role currentRole) 
        { 
            var existing = await _context.Users.FindAsync(dto.Id); 

            if (existing == null) 
                throw new Exception("User not found"); 

            if (currentRole == Role.Employee && existing.Id != currentUserId) 
                throw new UnauthorizedAccessException(); 

            existing.FirstName = dto.FirstName; 
            existing.LastName = dto.LastName; 
            existing.Username = dto.Username;
            
            if (!string.IsNullOrWhiteSpace(dto.Password)) 
            { 
                existing.Password = _passwordHasher.HashPassword(existing, dto.Password); 
            } 

            if (currentRole != Role.Employee) 
            { 
                existing.Role = dto.Role; 
            } 

            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteUserAsync(int id, int currentUserId, Role currentRole) 
        { 
            var user = await _context.Users.FindAsync(id); 
            if (user == null) 
                throw new Exception("User not found"); 

            if (currentRole == Role.Employee && user.Id != currentUserId) 
                throw new UnauthorizedAccessException(); 

            _context.Users.Remove(user); 
            await _context.SaveChangesAsync(); 
        }
    }
      
}
