using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _accountService.LoginAsync(loginDto);
            if (!result.Success) return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _accountService.RegisterAsync(registerDto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("CompleteProfile")]
        [Authorize] // تم إزالة (Roles = "Partial Registered") لقبول اليوزر حتى لو أصبح Student
        public async Task<IActionResult> CompleteProfile(CompleteProfileDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var result = await _accountService.CompleteProfileAsync(email, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}