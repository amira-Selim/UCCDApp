using UCCD_App.Dto;

namespace UCCD_App.Services
{
    public interface IStudentCourseService
    {
        // ميثود التسجيل (للطالب)
        Task<ApiResponse<EnrollmentResponseDto>> EnrollByEmailAsync(string email, int courseId);
        
        // ميثود عرض الكورسات (للطالب)
        Task<List<StudentEnrollmentDto>> GetEnrollmentsByEmailAsync(string email);
        
        // ميثود تحديث الحالة (للأدمن)
        Task<ApiResponse<string>> UpdateEnrollmentStatusAsync(UpdateEnrollmentStatusDto dto);
    }
}