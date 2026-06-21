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

        // ميثود عرض الطلاب المسجلين في كورس معين (للأدمن) - Course Details page
        Task<List<EnrolledStudentDto>> GetEnrolledStudentsAsync(int courseId);

        // ميثود عرض الكورسات اللي طالب معين مسجل فيها بالـ ID (للأدمن) - Student Details page
        Task<List<StudentRegisteredCourseDto>> GetCoursesForStudentAsync(int studentId);
    }
}