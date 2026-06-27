using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Dto;
using UCCD_App.Models;

namespace UCCD_App.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    public ProfileService(AppDbContext context) => _context = context;

    // 1. ميثود خاصة (Private) للـ Mapping عشان م نكررش الكود
    private StudentProfileDto MapToDto(Student student)
    {
        return new StudentProfileDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            Phone = student.Phone,
            Gender = student.Gender,
            Faculty = student.Faculty,
            GraduationYear = student.GraduationYear,
            NationalID = student.NationalID,
            Education = student.Education,
            Skills = student.Skills,
            Interests = student.Interests,
            CareerGoal = student.CareerGoal,
            EnrolledCoursesCount = student.StudentCourses?.Count ?? 0
        };
    }

    // 2. جلب بيانات البروفايل للطالب (بالإيميل)
    public async Task<ApiResponse<StudentProfileDto>> GetProfileDataAsync(string email)
    {
        var student = await _context.Students
            .Include(s => s.StudentCourses)
            .FirstOrDefaultAsync(s => s.Email == email);

        if (student == null) 
            return new ApiResponse<StudentProfileDto> { Success = false, Message = "Profile not found" };

        return new ApiResponse<StudentProfileDto> { Success = true, Data = MapToDto(student) };
    }

    // 3. تحديث البيانات الأساسية
    public async Task<ApiResponse<StudentProfileDto>> UpdateProfileAsync(string email, StudentProfileDto dto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        if (student == null) return new ApiResponse<StudentProfileDto> { Success = false, Message = "Student not found" };

        student.FullName = dto.FullName;
        student.Phone = dto.Phone;
        student.Faculty = dto.Faculty;
        student.GraduationYear = dto.GraduationYear;

        await _context.SaveChangesAsync();
        return await GetProfileDataAsync(email);
    }

    // 4. تحديث البيانات المهنية
    public async Task<ApiResponse<StudentProfileDto>> UpdateProfessionalInfoAsync(string email, StudentProfileDto dto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        if (student == null) return new ApiResponse<StudentProfileDto> { Success = false, Message = "Student not found" };

        student.Education = dto.Education;
        student.Skills = dto.Skills;
        student.Interests = dto.Interests;
        student.CareerGoal = dto.CareerGoal;

        await _context.SaveChangesAsync();
        return await GetProfileDataAsync(email);
    }

    // 5. جلب كل الطلاب (للأدمن) - بترجع كل البيانات بدون حسابات الأدمن
    public async Task<ApiResponse<IEnumerable<StudentProfileDto>>> GetAllStudentsAsync()
    {
        var adminEmails = await (from user in _context.Users
                                 join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where role.Name == "Admin"
                                 select user.Email).ToListAsync();

        var students = await _context.Students
            .Where(s => !adminEmails.Contains(s.Email))
            .Include(s => s.StudentCourses)
            .ToListAsync();

        var data = students.Select(s => MapToDto(s));

        return new ApiResponse<IEnumerable<StudentProfileDto>> { Success = true, Data = data };
    }

    // 6. جلب طالب معين بالـ ID (للأدمن) - بترجع كل البيانات
    public async Task<ApiResponse<StudentProfileDto>> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.StudentCourses)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return new ApiResponse<StudentProfileDto> { Success = false, Message = "Student not found" };

        return new ApiResponse<StudentProfileDto> { Success = true, Data = MapToDto(student) };
    }

    public async Task<ApiResponse<ApplicantProfileDto>> GetApplicantProfileAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
            .Include(s => s.VolunteerApplications)
                .ThenInclude(va => va.Opportunity)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return new ApiResponse<ApplicantProfileDto> { Success = false, Message = "Student not found" };

        var dto = new ApplicantProfileDto
        {
            StudentId = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            Phone = student.Phone,
            Faculty = student.Faculty ?? "",
            GraduationYear = student.GraduationYear ?? "",
            Education = student.Education ?? "",
            Skills = student.Skills ?? "",
            Interests = student.Interests ?? "",
            CareerGoal = student.CareerGoal ?? "",
            Courses = student.StudentCourses.Select(sc => new ApplicantCourseDto
            {
                CourseId = sc.CouresId,
                CourseName = sc.Course?.Name ?? "",
                Status = sc.StudentStatus.ToString()
            }).ToList(),
            VolunteerWork = student.VolunteerApplications.Select(va => new ApplicantVolunteerDto
            {
                VolunteerOpportunityId = va.OpportunityId,
                Title = va.Opportunity?.Title ?? "",
                Status = va.Status.ToString()
            }).ToList()
        };

        return new ApiResponse<ApplicantProfileDto> { Success = true, Data = dto };
    }
}