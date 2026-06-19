using UCCD_App.Dto;
using Microsoft.AspNetCore.Mvc;

namespace UCCD_App.Services
{
    public interface IAccountService
    {
        Task<ApiResponse<UserTokenResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<UserTokenResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<UserTokenResponseDto>> CompleteProfileAsync(string email, CompleteProfileDto dto);
    }
}