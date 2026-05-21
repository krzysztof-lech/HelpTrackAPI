using HelpTrackAPI.Models.Dtos;
using HelpTrackAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HelpTrackAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketMessagesController : ControllerBase
    {
        private readonly ITicketMessageService _service;

        public TicketMessagesController(ITicketMessageService service) 
        { 
            _service = service; 
        }

        [HttpGet("ticket/{ticketId}")]
        [ProducesResponseType(typeof(IEnumerable<ChatMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMessages(int ticketId) 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (userIdString is null || role is null || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var messages = await _service.GetMessagesAsync(ticketId, userId, role); 
            return Ok(messages); 
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostMessage(CreateChatMessageDto dto) 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (userIdString is null || role is null || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var message = await _service.AddMessageAsync(dto, userId, role); 
            return Ok(message); 
        }

        [HttpPatch("ticket/{ticketId}/mark-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MarkAsRead(int ticketId) 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (userIdString is null || role is null || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var count = await _service.MarkAsReadAsync(ticketId, userId, role); 
            return Ok(new { markedCount = count }); 
        }
    }


}