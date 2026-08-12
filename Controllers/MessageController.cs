using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly MessageRepository _messageRepository;
        public MessagesController(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        // POST: api/messages  { recipientIds: [1,2,3], content: "..." }
        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            foreach (var recipientId in request.RecipientIds)
            {
                await _messageRepository.Send(senderId, recipientId, request.Content);
            }
            return Ok();
        }

        // GET: api/messages/inbox
        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var messages = await _messageRepository.GetInbox(userId);
            return Ok(messages);
        }

        // GET: api/messages/unread-count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var count = await _messageRepository.GetUnreadCount(userId);
            return Ok(new { count });
        }

        // PUT: api/messages/5/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _messageRepository.MarkRead(id);
            return NoContent();
        }
    }
}