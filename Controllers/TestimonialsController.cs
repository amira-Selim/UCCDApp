using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UCCD_App.Context;
using UCCD_App.Models;

namespace UCCD_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TestimonialsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedTestimonials()
        {
            var testimonials = await _context.Testimonials
                .Where(t => t.IsApproved)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(testimonials);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitTestimonial([FromBody] Testimonial dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var testimonial = new Testimonial
            {
                Name = dto.Name,
                Role = dto.Role,
                Content = dto.Content,
                AvatarUrl = dto.AvatarUrl,
                IsApproved = false 
            };
            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Testimonial submitted successfully and is pending approval." });
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTestimonials()
        {
            var testimonials = await _context.Testimonials
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(testimonials);
        }

        [HttpPut("admin/approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveTestimonial(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null) return NotFound(new { message = "Testimonial not found." });
            testimonial.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Testimonial approved successfully." });
        }
        
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTestimonial(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null) return NotFound(new { message = "Testimonial not found." });
            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Testimonial deleted successfully." });
        }
    }
}