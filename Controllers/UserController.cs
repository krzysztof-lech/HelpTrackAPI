using HelpTrackAPI.Data;
using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        // GET: api/User
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers() 
        { 
            var currentUserId = GetCurrentUserId(); 
            var currentRole = GetCurrentUserRole(); 
            var users = await _userService.GetUsersAsync(currentUserId, currentRole); 
            return Ok(users); 
        }

        // GET: api/User/5
        [HttpGet("{id}")] 
        public async Task<ActionResult<UserDto>> GetUser(int id) 
        {
            var currentUserId = GetCurrentUserId(); 
            var currentRole = GetCurrentUserRole(); 
            var user = await _userService.GetUserByIdAsync(id, currentUserId, currentRole); 

            if (user == null) 
                return NotFound(); 
            
            return Ok(user); 
        }

        // POST: api/User
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> PostUser([FromBody] CreateUserDto dto) 
        { 
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var created = await _userService.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);

        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UpdateUserDto dto) 
        { 
            if (!ModelState.IsValid) 
                return BadRequest(ModelState); 
            
            if (id != dto.Id) 
                return BadRequest("Id mismatch"); 
            
            var currentUserId = GetCurrentUserId(); 
            var currentRole = GetCurrentUserRole(); 
            
            try 
            { 
                await _userService.UpdateUserAsync(dto, currentUserId, currentRole); 
            } 
            catch (UnauthorizedAccessException) 
            { 
                return Forbid(); 
            } 
            catch (Exception) 
            { 
                return NotFound(); 
            } 

            return NoContent(); 
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id) 
        { 
            var currentUserId = GetCurrentUserId(); 
            var currentRole = GetCurrentUserRole(); 

            try 
            { 
                await _userService.DeleteUserAsync(id, currentUserId, currentRole); 
            } 
            catch (UnauthorizedAccessException) 
            { 
                return Forbid(); 
            } 
            catch (Exception) 
            { 
                return NotFound(); 
            } 

            return NoContent(); 
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : 0;
        }

        private Role GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim != null && Enum.TryParse(roleClaim, out Role role) ? role : Role.Employee;
        }
    }
}
