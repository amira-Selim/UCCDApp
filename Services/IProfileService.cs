using UCCD_App.Dto;

namespace UCCD_App.Services
{
    public interface IProfileService
    {Task<ApiResponse<IEnumerable<StudentProfileDto>>> GetAllStudentsAsync();
    Task<ApiResponse<StudentProfileDto>> GetStudentByIdAsync(int id); // الدالة الجديدة
    Task<ApiResponse<StudentProfileDto>> GetProfileDataAsync(string email);
    Task<ApiResponse<StudentProfileDto>> UpdateProfileAsync(string email, StudentProfileDto dto);
    Task<ApiResponse<StudentProfileDto>> UpdateProfessionalInfoAsync(string email, StudentProfileDto dto);
    Task<ApiResponse<ApplicantProfileDto>> GetApplicantProfileAsync(int studentId);
    }
}