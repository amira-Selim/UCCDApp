using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Dto;
using UCCD_App.Models;

namespace UCCD_App.Services
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public StudentCourseService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

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

            // We will use INotificationService after SaveChangesAsync to ensure course exists and signalr triggers
            // _context.Notifications.Add(notification); // Removed direct add

            await _context.SaveChangesAsync();

            // Real-time notification to Admins
            await _notificationService.CreateNotificationAsync(
                "New Enrollment Request",
                $"{student.FullName} submitted an enrollment request for {course.Name}.",
                "Info",
                null,
                courseId
            );

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

            if (dto.Status == StudentStatus.Aproved || dto.Status == StudentStatus.Rejected)
            {
                var student = await _context.Students.FindAsync(dto.StudentId);
                var course = await _context.Courses.FindAsync(dto.CourseId);
                
                // Get ApplicationUser to use correct string GUID for UserId instead of student's int Id
                var appUser = student != null ? await _context.Users.FirstOrDefaultAsync(u => u.Email == student.Email) : null;
                
                string statusWord = dto.Status == StudentStatus.Aproved ? "approved" : "rejected";
                string typeStr = dto.Status == StudentStatus.Aproved ? "Success" : "Error";
                // We'll call INotificationService after SaveChanges
            }

            await _context.SaveChangesAsync();

            if (dto.Status == StudentStatus.Aproved || dto.Status == StudentStatus.Rejected || dto.Status == StudentStatus.Completed)
            {
                var student = await _context.Students.FindAsync(dto.StudentId);
                var course = await _context.Courses.FindAsync(dto.CourseId);
                var appUser = student != null ? await _context.Users.FirstOrDefaultAsync(u => u.Email == student.Email) : null;
                
                string statusWord = dto.Status == StudentStatus.Aproved ? "approved" 
                                  : dto.Status == StudentStatus.Completed ? "marked as completed" 
                                  : "rejected";
                string typeStr = (dto.Status == StudentStatus.Aproved || dto.Status == StudentStatus.Completed) ? "Success" : "Error";
                
                if (appUser != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        $"Course {dto.Status}",
                        $"Your enrollment in {course?.Name} has been {statusWord}.",
                        typeStr,
                        appUser.Id,
                        dto.CourseId
                    );
                }
            }

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

        public async Task<ApiResponse<string>> CancelEnrollmentAsync(string email, int courseId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null) return new ApiResponse<string> { Success = false, Message = "Student not found." };

            var enrollment = await _context.StudentCourses
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentId == student.Id && sc.CouresId == courseId);

            if (enrollment == null)
                return new ApiResponse<string> { Success = false, Message = "Enrollment not found." };

            if (enrollment.StudentStatus == StudentStatus.Aproved || enrollment.StudentStatus == StudentStatus.Completed)
                return new ApiResponse<string> { Success = false, Message = "لا يمكنك إلغاء أو حذف طلب تم قبوله أو إكماله." };

            var courseName = enrollment.Course?.Name ?? "a course";

            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                "Enrollment Cancelled",
                $"Student {student.FullName} has cancelled their enrollment in {courseName}.",
                "Warning",
                null, // Admin
                courseId,
                null,
                null
            );

            return new ApiResponse<string> { Success = true, Message = "تم إزالة الطلب بنجاح." };
        }
    }
}