using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentCourseController : ControllerBase
    {
        private readonly IStudentCourseService _service;

        public StudentCourseController(IStudentCourseService service)
        {
            _service = service;
        }

        private string? UserEmail => User.FindFirstValue(ClaimTypes.Email);

        [Authorize(Roles = "Student")]
        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _service.EnrollByEmailAsync(UserEmail, courseId);
            
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("my-enrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _service.GetEnrollmentsByEmailAsync(UserEmail);
            return Ok(new ApiResponse<List<StudentEnrollmentDto>> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateEnrollmentStatusDto dto)
        {
            var result = await _service.UpdateEnrollmentStatusAsync(dto);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        
    }
}