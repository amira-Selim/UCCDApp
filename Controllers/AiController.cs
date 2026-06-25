using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure user is logged in
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        private string? UserEmail => User.FindFirstValue(ClaimTypes.Email);

        [HttpPost("cover-letter")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GenerateCoverLetter([FromBody] AiCoverLetterRequestDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ApiResponse<string> { Success = false, Message = "User email not found in token." });

            var result = await _aiService.GenerateCoverLetterAsync(UserEmail, dto.JobId);

            if (result.ResultText.StartsWith("Error"))
                return BadRequest(new ApiResponse<string> { Success = false, Message = result.ResultText });

            return Ok(new ApiResponse<AiResponseDto> { Success = true, Data = result, Message = "Cover letter generated successfully." });
        }

        [HttpGet("recommendations")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetRecommendations()
        {
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ApiResponse<string> { Success = false, Message = "User email not found in token." });

            var result = await _aiService.GetRecommendedJobsAsync(UserEmail);

            return Ok(new ApiResponse<object> { Success = true, Data = result, Message = "Recommendations fetched successfully." });
        }

        [HttpPost("recommendations/courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseRecommendations([FromBody] AiCourseRecommendationRequestDto request)
        {
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ApiResponse<string> { Success = false, Message = "User email not found in token." });

            var result = await _aiService.GetRecommendedCoursesAsync(UserEmail, request);

            return Ok(new ApiResponse<object> { Success = true, Data = result, Message = "Course recommendations fetched successfully." });
        }
    }
}
