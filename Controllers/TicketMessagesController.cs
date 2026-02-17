using HelpTrackAPI.Data;
using HelpTrackAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketMessagesController : ControllerBase
    {
        private readonly HelpTrackContext _context;

        public TicketMessagesController(HelpTrackContext context)
        {
            _context = context;
        }

        
        [HttpGet("ticket/{ticketId}")]
        public async Task<ActionResult<IEnumerable<TicketMessage>>> GetMessages(int ticketId)
        {
            return await _context.TicketMessages
                .Include(m => m.Author) 
                .Where(m => m.TicketId == ticketId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        
        [HttpPost]
        public async Task<ActionResult<TicketMessage>> PostMessage(TicketMessage message)
        {
            message.CreatedAt = DateTime.Now;
            message.IsRead = false;

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();

            var result = await _context.TicketMessages
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == message.Id);

            return Ok(result);
        }

        [HttpPatch("ticket/{ticketId}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int ticketId, [FromBody] MarkReadRequest request)
        {
            var messages = await _context.TicketMessages
                .Where(m => m.TicketId == ticketId && m.AuthorId != request.UserId && !m.IsRead)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { markedCount = messages.Count });
        }
    }

    public class MarkReadRequest
    {
        public int UserId { get; set; }
    }

}