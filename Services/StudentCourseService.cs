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

            // أي تسجيل جديد بيبقى Pending دايمًا - الأدمن هو اللي يقرر يوافق أو يرفض
            var status = StudentStatus.Pending;

            var enrollment = new StudentCourse
            {
                StudentId = student.Id,
                CouresId = courseId,
                StudentStatus = status,
                EnrolledAt = DateTime.UtcNow
            };

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            // لو الكورس مدفوع، نوجّه الطالب يروح المركز يدفع ويأكد التسجيل
            var message = course.Price > 0
                ? "تم تسجيلك بنجاح، طلبك الآن قيد المراجعة. برجاء التوجه إلى المركز لدفع رسوم الكورس وتأكيد التسجيل."
                : "تم تسجيلك بنجاح، طلبك الآن قيد المراجعة لحين موافقة الإدارة.";

            return new ApiResponse<EnrollmentResponseDto>
            {
                Success = true,
                Message = message,
                Data = new EnrollmentResponseDto
                {
                    StudentId = student.Id,
                    Status = status.ToString(),
                    RequiresPayment = course.Price > 0,
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

            // مينفعش حد يتعمله Completed إلا لو كان Approved الأول
            if (dto.Status == StudentStatus.Completed && enrollment.StudentStatus != StudentStatus.Aproved)
                return new ApiResponse<string> { Success = false, Message = "لازم يكون الطالب Approved الأول قبل ما يتحوّل لـ Completed" };

            enrollment.StudentStatus = dto.Status;

            if (dto.Status == StudentStatus.Completed && enrollment.CompletedAt == null)
                enrollment.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse<string> { Success = true, Message = "تم تحديث حالة الطالب بنجاح" };
        }

        // Course Details page (Admin): all students enrolled in a given course
        public async Task<List<EnrolledStudentDto>> GetEnrolledStudentsAsync(int courseId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.CouresId == courseId)
                .Include(sc => sc.Student)
                .Select(sc => new EnrolledStudentDto
                {
                    StudentId = sc.StudentId,
                    FullName = sc.Student!.FullName,
                    Email = sc.Student.Email,
                    Phone = sc.Student.Phone,
                    EnrollmentDate = sc.EnrolledAt,
                    Status = sc.StudentStatus.ToString()
                })
                .ToListAsync();
        }

        // Student Details panel (Admin): all courses a given student (by ID) is enrolled in
        public async Task<List<StudentRegisteredCourseDto>> GetCoursesForStudentAsync(int studentId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Course)
                .Select(sc => new StudentRegisteredCourseDto
                {
                    CourseId = sc.CouresId,
                    CourseName = sc.Course!.Name,
                    Instructor = sc.Course.Instructor,
                    StartDate = sc.Course.StartDate,
                    Status = sc.StudentStatus.ToString()
                })
                .ToListAsync();
        }
    }
}