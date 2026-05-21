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
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers() 
        { 
            var currentUserId = GetCurrentUserId(); 
            var currentRole = GetCurrentUserRole(); 
            var users = await _userService.GetUsersAsync(currentUserId, currentRole); 
            return Ok(users); 
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserDto>> PostUser([FromBody] CreateUserDto dto) 
        { 
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var created = await _userService.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);

        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent(); 
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent(); 
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException();
            return userId;
        }

        private Role GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim != null && Enum.TryParse(roleClaim, out Role role) ? role : Role.Employee;
        }
    }
}
