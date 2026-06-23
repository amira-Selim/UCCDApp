using Microsoft.AspNetCore.Mvc;
using UCCD_App.Dto;
using UCCD_App.Services;
using Microsoft.AspNetCore.Authorization;

namespace UCCD_App.Controllers

{
    [ApiController]
    [Route("api/messages")]

   
    public class ContactController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ContactController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // POST: api/messages
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMessageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _messageService.CreateAsync(dto);

            return Ok(result);
        }

        // GET: api/messages
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _messageService.GetAllAsync();
            return Ok(messages);
        }

        // GET: api/messages/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var message = await _messageService.GetByIdAsync(id);

            if (message == null)
                return NotFound("Message not found");

            return Ok(message);
        }

        // PUT: api/messages/{id}/read
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _messageService.MarkAsReadAsync(id);

            if (!result)
                return NotFound("Message not found");

            return Ok(new { message = "Marked as read" });
        }

        // DELETE: api/messages/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _messageService.DeleteAsync(id);

            if (!result)
                return NotFound("Message not found");

            return Ok(new { message = "Deleted successfully" });
        }
    }
}