using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UCCD_App.Context;

namespace UCCD_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var studentsCount = await _context.Students.CountAsync();
            var coursesCount = await _context.Courses.CountAsync();
            var jobsCount = await _context.JobOpportunities.CountAsync();
            var volunteersCount = await _context.VolunteerOpportunities.CountAsync();
            var messagesCount = await _context.Messages.CountAsync();
            var unreadMessagesCount = await _context.Messages.CountAsync(m => !m.IsRead);
            var totalJobApplicants = await _context.JobApplications.CountAsync();
            
            var facultyBreakdown = await _context.Students
                .GroupBy(s => s.Faculty)
                .Select(g => new { Faculty = g.Key, Count = g.Count() })
                .ToListAsync();
                
            var courseTypeBreakdown = await _context.Courses
                .GroupBy(c => c.Type)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                studentsCount,
                coursesCount,
                jobsCount,
                volunteerOpportunitiesCount = volunteersCount,
                messagesCount,
                unreadMessagesCount,
                totalJobApplicants,
                facultyBreakdown,
                courseTypeBreakdown
            });
        }
    }
}