using HelpTrackAPI.Data;
using HelpTrackAPI.Models;
using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/Ticket
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets([FromQuery] bool onlyMine = false)
        {
            var tickets = await _ticketService.GetTicketsAsync(
                GetCurrentUserId(), 
                GetCurrentUserRole(),
                onlyMine
            ); 
            
            return Ok(tickets);
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TicketDto>> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(
                id, 
                GetCurrentUserId(), 
                GetCurrentUserRole()); 
            
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket with ID {id} does not exist.");

            return Ok(ticket);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByUserId(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserIdAsync(
                userId,
                GetCurrentUserId(),
                GetCurrentUserRole()
            );

            return Ok(tickets);
        }


        // POST: api/Ticket
        [HttpPost]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TicketDto>> PostTicket([FromBody] CreateTicketDto dto)
        {
            var created = await _ticketService.CreateTicketAsync(dto, GetCurrentUserId()); 
            
            return CreatedAtAction(nameof(GetTicket), new { id = created.Id }, created);
        }


        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PutTicket(int id, [FromBody] UpdateTicketDto updateDto)
        {
            await _ticketService.UpdateTicketAsync(
                id,
                updateDto,
                GetCurrentUserId(),
                GetCurrentUserRole()
            );

            return NoContent();
        }


        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _ticketService.DeleteTicketAsync(
                id,
                GetCurrentUserId(),
                GetCurrentUserRole()
            );

            return NoContent();
        }


        [HttpPatch("{id}/status")]
        [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.SupportAgent))]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusDto dto)
        {
            var updated = await _ticketService.UpdateTicketStatusAsync(
                id, 
                dto, 
                GetCurrentUserId(), 
                GetCurrentUserRole()
            ); 
            
            return Ok(updated);
        }


        [HttpPatch("{id}/assign")]
        [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.SupportAgent))]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignTicketDto dto)
        {
            var updated = await _ticketService.AssignTicketAsync(
                id, 
                dto, 
                GetCurrentUserId(), 
                GetCurrentUserRole()
            ); 
            
            return Ok(updated);
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
