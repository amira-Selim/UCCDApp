using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using UCCD_App.Context;
using UCCD_App.Models;
using UCCD_App.Hubs;

namespace UCCD_App.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task CreateNotificationAsync(string title, string message, string type = "Info", string? userId = null, int? relatedCourseId = null, int? relatedVolunteerId = null, int? relatedJobId = null, string? recipientEmail = null, string? recipientRole = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                UserId = userId,
                RelatedCourseId = relatedCourseId,
                RelatedVolunteerId = relatedVolunteerId,
                RelatedJobId = relatedJobId,
                RecipientEmail = recipientEmail,
                RecipientRole = recipientRole,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Broadcast the notification to all connected clients
            // The frontend will decide whether to display it based on the user's role
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}