using UCCD_App.Dto;
using Microsoft.AspNetCore.Mvc;

namespace UCCD_App.Services
{
    public interface IAccountService
    {
        Task<ApiResponse<UserTokenResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<UserTokenResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<UserTokenResponseDto>> CompleteProfileAsync(string email, CompleteProfileDto dto);
        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(string email, ChangePasswordDto dto);
    }
}