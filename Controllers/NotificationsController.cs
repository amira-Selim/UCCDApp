using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UCCD_App.Context;
using UCCD_App.Models;
using System.Security.Claims;

namespace UCCD_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            
            var query = _context.Notifications.AsQueryable();
            
            query = query.Where(n => 
                n.UserId == userId || 
                n.RecipientEmail == email || 
                roles.Contains(n.RecipientRole) || 
                (roles.Contains("Admin") && n.UserId == null && n.RecipientEmail == null && n.RecipientRole == null)
            );
            
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();
                
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var count = await _context.Notifications.CountAsync(n => 
                !n.IsRead && 
                (n.UserId == userId || 
                 n.RecipientEmail == email || 
                 roles.Contains(n.RecipientRole) || 
                 (roles.Contains("Admin") && n.UserId == null && n.RecipientEmail == null && n.RecipientRole == null))
            );

            return Ok(new { count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound(new { message = "Notification not found." });
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            bool isAuthorized = notification.UserId == userId || 
                                notification.RecipientEmail == email || 
                                roles.Contains(notification.RecipientRole) || 
                                roles.Contains("Admin");

            if (!isAuthorized) return Forbid();

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Notification marked as read." });
        }
        
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var unreadNotifications = await _context.Notifications
                .Where(n => !n.IsRead && 
                            (n.UserId == userId || 
                             n.RecipientEmail == email || 
                             roles.Contains(n.RecipientRole) || 
                             (roles.Contains("Admin") && n.UserId == null && n.RecipientEmail == null && n.RecipientRole == null)))
                .ToListAsync();

            foreach(var n in unreadNotifications)
            {
                n.IsRead = true;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "All notifications marked as read." });
        }
    }
}