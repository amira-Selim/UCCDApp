using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Dto;
using UCCD_App.Models;

namespace UCCD_App.Services
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly AppDbContext _context;
        public StudentCourseService(AppDbContext context) => _context = context;

        public async Task<ApiResponse<EnrollmentResponseDto>> EnrollByEmailAsync(string email, int courseId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null) 
                return new ApiResponse<EnrollmentResponseDto> { Success = false, Message = "برجاء إكمال بيانات البروفايل أولاً" };

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) 
                return new ApiResponse<EnrollmentResponseDto> { Success = false, Message = "الكورس غير موجود" };

            var exists = await _context.StudentCourses.AnyAsync(sc => sc.StudentId == student.Id && sc.CouresId == courseId);
            if (exists) 
                return new ApiResponse<EnrollmentResponseDto> { Success = false, Message = "أنت مسجل بالفعل في هذا الكورس" };

            var approvedCount = await _context.StudentCourses
                .CountAsync(sc => sc.CouresId == courseId && sc.StudentStatus == StudentStatus.Aproved);

            var status = approvedCount < course.Capacity ? StudentStatus.Aproved : StudentStatus.Rejected;

            var enrollment = new StudentCourse
            {
                StudentId = student.Id,
                CouresId = courseId,
                StudentStatus = status
            };

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            return new ApiResponse<EnrollmentResponseDto> 
            { 
                Success = true, 
                Data = new EnrollmentResponseDto 
                { 
                    StudentId = student.Id,
                    Status = status.ToString(),
                    CourseDetails = new CourseDto 
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Price = course.Price,
                        Capacity = course.Capacity
                    }
                } 
            };
        }

        public async Task<List<StudentEnrollmentDto>> GetEnrollmentsByEmailAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null) return new List<StudentEnrollmentDto>();

            return await _context.StudentCourses
                .Where(sc => sc.StudentId == student.Id)
                .Include(sc => sc.Course)
                .Select(sc => new StudentEnrollmentDto 
                { 
                    StudentId = sc.StudentId,
                    CourseId = sc.CouresId,
                    Status = sc.StudentStatus.ToString(),
                    CourseDetails = new CourseDto 
                    {
                        Id = sc.Course!.Id,
                        Name = sc.Course.Name,
                        Price = sc.Course.Price,
                        Capacity = sc.Course.Capacity
                    }
                }).ToListAsync();
        }

        public async Task<ApiResponse<string>> UpdateEnrollmentStatusAsync(UpdateEnrollmentStatusDto dto)
        {
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == dto.StudentId && sc.CouresId == dto.CourseId);

            if (enrollment == null) 
                return new ApiResponse<string> { Success = false, Message = "التسجيل غير موجود" };

            enrollment.StudentStatus = dto.Status;
            await _context.SaveChangesAsync();
            
            return new ApiResponse<string> { Success = true, Message = "تم تحديث حالة الطالب بنجاح" };
        }
    }
}