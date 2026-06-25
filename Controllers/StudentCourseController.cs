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

        // GET /api/StudentCourse/course/5/students
        // Used by the Admin "Course Details" page to list everyone enrolled in a course.
        [Authorize(Roles = "Admin")]
        [HttpGet("course/{courseId:int}/students")]
        public async Task<IActionResult> GetEnrolledStudents(int courseId)
        {
            var result = await _service.GetEnrolledStudentsAsync(courseId);
            return Ok(new ApiResponse<List<EnrolledStudentDto>> { Success = true, Data = result });
        }

        // GET /api/StudentCourse/student/5/courses
        // Used by the Admin "Student Details" panel to list the courses a student registered for.
        [Authorize(Roles = "Admin")]
        [HttpGet("student/{studentId:int}/courses")]
        public async Task<IActionResult> GetCoursesForStudent(int studentId)
        {
            var result = await _service.GetCoursesForStudentAsync(studentId);
            return Ok(new ApiResponse<List<StudentRegisteredCourseDto>> { Success = true, Data = result });
        }

        [Authorize(Roles = "Student")]
        [HttpDelete("cancel/{courseId}")]
        public async Task<IActionResult> CancelEnrollment(int courseId)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _service.CancelEnrollmentAsync(UserEmail, courseId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }



    }
}