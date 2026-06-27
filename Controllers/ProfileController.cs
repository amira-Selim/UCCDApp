using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")] 
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService) => _profileService = profileService;

        private string? UserEmail => User.FindFirstValue(ClaimTypes.Email);

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _profileService.GetProfileDataAsync(UserEmail);
            if (!result.Success) return NotFound(result);

            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(StudentProfileDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _profileService.UpdateProfileAsync(UserEmail, dto);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("update-professional-info")]
        public async Task<IActionResult> UpdateProfessionalInfo(StudentProfileDto dto)
        {
            if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

            var result = await _profileService.UpdateProfessionalInfoAsync(UserEmail, dto);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("applicant/{studentId}")]
        [Authorize(Roles = "Admin,Company")]
        public async Task<IActionResult> GetApplicantProfile(int studentId)
        {
            var result = await _profileService.GetApplicantProfileAsync(studentId);
            if (!result.Success) return NotFound(result);

            return Ok(result);
        }
    }
}