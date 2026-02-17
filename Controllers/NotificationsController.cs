using HelpTrackAPI.Models;
using HelpTrackAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpTrackAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly HelpTrackContext _context;

        public NotificationsController(HelpTrackContext context)
        {
            _context = context;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            if (!int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var userId))
                return BadRequest("Could not read user ID.");

     
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            if (!notifications.Any())
            {
                var welcome = new Notification
                {
                    UserId = userId,
                    Message = "Witaj w HelpTrack. Tutaj będziesz widzieć swoje powiadomienia.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(welcome);
                await _context.SaveChangesAsync();

                notifications.Add(welcome);
            }

            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
