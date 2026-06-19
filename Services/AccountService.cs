using Microsoft.AspNetCore.Identity;
using UCCD_App.Dto;
using UCCD_App.Models;
using UCCD_App.Repo;

namespace UCCD_App.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepo<Student> _studentRepo;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<ApplicationUser> userManager, IGenericRepo<Student> studentRepo, IConfiguration configuration)
        {
            _userManager = userManager;
            _studentRepo = studentRepo;
            _configuration = configuration;
        }

        public async Task<ApiResponse<UserTokenResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new ApiResponse<UserTokenResponseDto> { Success = false, Message = "Email Or Password Wrong" };

            var token = await TokenService.GenerateTokenAsync(user, _userManager, _configuration);
            return new ApiResponse<UserTokenResponseDto> { Success = true, Data = new UserTokenResponseDto { FullName = user.FullName, Email = user.Email!, Token = token } };
        }

        public async Task<ApiResponse<UserTokenResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                return new ApiResponse<UserTokenResponseDto> { Success = false, Message = "Email is already taken" };

            var user = new ApplicationUser
            {
                FullName = $"{registerDto.FirstName} {registerDto.LastName}",
                Email = registerDto.Email,
                UserName = registerDto.Email.Split('@').First(),
                PhoneNumber = registerDto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new ApiResponse<UserTokenResponseDto> { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await _userManager.AddToRoleAsync(user, "Partial Registered");

            var token = await TokenService.GenerateTokenAsync(user, _userManager, _configuration);
            return new ApiResponse<UserTokenResponseDto> { Success = true, Data = new UserTokenResponseDto { FullName = user.FullName, Email = user.Email!, Token = token } };
        }

        public async Task<ApiResponse<UserTokenResponseDto>> CompleteProfileAsync(string email, CompleteProfileDto dto)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserTokenResponseDto> { Success = false, Message = "User not found" };

            // --- التعديل الجديد: إذا كان اليوزر "Student" بالفعل، نرجع له التوكن فوراً ---
            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                var token = await TokenService.GenerateTokenAsync(user, _userManager, _configuration);
                return new ApiResponse<UserTokenResponseDto> 
                { 
                    Success = true, 
                    Message = "Profile already completed",
                    Data = new UserTokenResponseDto { FullName = user.FullName, Email = user.Email!, Token = token } 
                };
            }

            // تحديث بيانات الـ Identity User
            user.Gender = dto.Gender;
            user.Faculty = dto.Faculty;
            user.NationalID = dto.NationalID;
            user.GraduationYear = dto.GraduationYear;
            await _userManager.UpdateAsync(user);

            // تحويل الـ Role
            await _userManager.RemoveFromRoleAsync(user, "Partial Registered");
            await _userManager.AddToRoleAsync(user, "Student");

            // إضافة البيانات لجدول الـ Student
            await _studentRepo.AddAsync(new Student
            {
                Email = user.Email!,
                FullName = user.FullName,
                Phone = user.PhoneNumber!,
                Gender = user.Gender,
                Faculty = user.Faculty,
                NationalID = user.NationalID,
                GraduationYear = user.GraduationYear
            });

            var newToken = await TokenService.GenerateTokenAsync(user, _userManager, _configuration);
            return new ApiResponse<UserTokenResponseDto> { Success = true, Data = new UserTokenResponseDto { FullName = user.FullName, Email = user.Email!, Token = newToken } };
        }
    }
}