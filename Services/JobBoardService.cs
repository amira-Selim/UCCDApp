using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Dto;
using UCCD_App.Models;
using UCCD_App.Repo;

namespace UCCD_App.Services;

public class JobBoardService : IJobBoardService
{
    private readonly IGenericRepo<JobOpportunity> _jobRepo;
    private readonly IGenericRepo<JobApplication> _applicationRepo;
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;

    public JobBoardService(
        IGenericRepo<JobOpportunity> jobRepo,
        IGenericRepo<JobApplication> applicationRepo,
        AppDbContext context,
        IEmailService emailService,
        INotificationService notificationService,
        Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
    {
        _jobRepo = jobRepo;
        _applicationRepo = applicationRepo;
        _context = context;
        _emailService = emailService;
        _notificationService = notificationService;
        _webHostEnvironment = webHostEnvironment;
    }

    // 1. الأدمن بيضيف الوظيفة بنفسه (تنزل معتمدة ونشطة فوراً للطلاب)
    public async Task<ApiResponse<JobOpportunityResponseDto>> CreateJobOpportunityByAdminAsync(CreateJobOpportunityDto dto)
    {
        var job = new JobOpportunity
        {
            Title = dto.Title,
            CompanyName = dto.CompanyName,
            CompanyEmail = dto.CompanyEmail,
            Description = dto.Description,
            Requirements = dto.Requirements,
            Location = dto.Location,
            SalaryRange = dto.SalaryRange,
            Type = Enum.TryParse<JobType>(dto.Type, true, out var t) ? t : JobType.FullTime,
            TargetFaculty = dto.TargetFaculty,
            IsApproved = true // معتمدة فوراً لأن الأدمن هو اللي كاتبها
        };

        await _jobRepo.AddAsync(job);

        var responseDto = new JobOpportunityResponseDto
        {
            Id = job.Id,
            Title = job.Title,
            CompanyName = job.CompanyName,
            CompanyEmail = job.CompanyEmail,
            Description = job.Description,
            Requirements = job.Requirements,
            Location = job.Location,
            SalaryRange = job.SalaryRange,
            Type = job.Type.ToString(),
            TargetFaculty = job.TargetFaculty,
            IsApproved = job.IsApproved,
            CreatedAt = job.CreatedAt,
            Deadline = job.Deadline,
            TotalApplicants = 0
        };

        return new ApiResponse<JobOpportunityResponseDto>
        {
            Success = true,
            Message = "Job opportunity created and published to students successfully.",
            Data = responseDto
        };
    }

    // 2. الطالب يعرض الوظائف المتاحة (مع الفلترة الذكية لكليته)
    public async Task<ApiResponse<IEnumerable<JobOpportunityResponseDto>>> GetApprovedJobsForStudentsAsync(string studentEmail, bool filterByMyFacultyOnly)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);

        var query = _context.JobOpportunities.Where(j => j.IsApproved && (j.Deadline == null || j.Deadline > DateTime.UtcNow));

        if (filterByMyFacultyOnly && student != null && !string.IsNullOrWhiteSpace(student.Faculty))
        {
            query = query.Where(j => j.TargetFaculty.ToLower() == student.Faculty.ToLower());
        }

        var jobs = await query.ToListAsync();
        var result = jobs.Select(j => new JobOpportunityResponseDto
        {
            Id = j.Id,
            Title = j.Title,
            CompanyName = j.CompanyName,
            CompanyEmail = j.CompanyEmail,
            Description = j.Description,
            Requirements = j.Requirements,
            Location = j.Location,
            SalaryRange = j.SalaryRange,
            Type = j.Type.ToString(),
            TargetFaculty = j.TargetFaculty,
            IsApproved = j.IsApproved,
            CreatedAt = j.CreatedAt,
            Deadline = j.Deadline
        }).ToList();

        return new ApiResponse<IEnumerable<JobOpportunityResponseDto>> { Success = true, Data = result };
    }

    // 3. عرض تفاصيل وظيفة محددة
    public async Task<ApiResponse<JobOpportunityResponseDto>> GetJobByIdAsync(int id)
    {
        var job = await _jobRepo.GetByIdAsync(id);
        if (job == null)
            return new ApiResponse<JobOpportunityResponseDto> { Success = false, Message = "Job opportunity not found." };

        var count = await _context.JobApplications.CountAsync(a => a.JobOpportunityId == id);

        var dto = new JobOpportunityResponseDto
        {
            Id = job.Id,
            Title = job.Title,
            CompanyName = job.CompanyName,
            CompanyEmail = job.CompanyEmail,
            Description = job.Description,
            Requirements = job.Requirements,
            Location = job.Location,
            SalaryRange = job.SalaryRange,
            Type = job.Type.ToString(),
            TargetFaculty = job.TargetFaculty,
            IsApproved = job.IsApproved,
            CreatedAt = job.CreatedAt,
            Deadline = job.Deadline,
            TotalApplicants = count
        };

        return new ApiResponse<JobOpportunityResponseDto> { Success = true, Data = dto };
    }

    // 4. تقديم الطالب على الوظيفة وإرسال إيميل الإشعار التلقائي
    public async Task<ApiResponse<JobApplicationResponseDto>> ApplyForJobAsync(string studentEmail, int jobId, ApplyJobDto dto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
        if (student == null)
            return new ApiResponse<JobApplicationResponseDto> { Success = false, Message = "Please complete your student profile first." };

        var job = await _jobRepo.GetByIdAsync(jobId);
        if (job == null)
            return new ApiResponse<JobApplicationResponseDto> { Success = false, Message = "Job opportunity not found." };

        var alreadyApplied = await _context.JobApplications.AnyAsync(a => a.JobOpportunityId == jobId && a.StudentId == student.Id);
        if (alreadyApplied)
            return new ApiResponse<JobApplicationResponseDto> { Success = false, Message = "You have already applied for this vacancy." };

        if (dto.CvFile == null || dto.CvFile.Length == 0)
            return new ApiResponse<JobApplicationResponseDto> { Success = false, Message = "CV File is required." };

        // 1. Validate File Size (Max 5MB)
        if (dto.CvFile.Length > 5 * 1024 * 1024)
            return new ApiResponse<JobApplicationResponseDto> { Success = false, Message = "CV File size exceeds the 5MB limit." };

        // 2. Validate File Extension
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var extension = System.IO.Path.GetExtension(dto.CvFile.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return new ApiResponse<JobApplicationResponseDto> { Success = false, Message = "Invalid CV File format. Only PDF, DOC, and DOCX are allowed." };

        // 3. Prepare Upload Directory
        var webRootPath = _webHostEnvironment.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
        var uploadPath = System.IO.Path.Combine(webRootPath, "uploads", "cvs");
        if (!System.IO.Directory.Exists(uploadPath))
        {
            System.IO.Directory.CreateDirectory(uploadPath);
        }

        // 4. Save File
        var uniqueFileName = Guid.NewGuid().ToString() + extension;
        var fullPath = System.IO.Path.Combine(uploadPath, uniqueFileName);
        using (var stream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
        {
            await dto.CvFile.CopyToAsync(stream);
        }

        var finalCvPath = $"/uploads/cvs/{uniqueFileName}";

        var application = new JobApplication
        {
            JobOpportunityId = jobId,
            StudentId = student.Id,
            CvFilePath = finalCvPath,
            CoverLetter = dto.CoverLetter,
            AppliedAt = DateTime.UtcNow
        };

        await _applicationRepo.AddAsync(application);

        // صياغة الإيميل التلقائي
        string emailSubject = $"New Application for {job.Title} - UCCD Portal";
        string emailBody = $@"
            <h3>Dear Admin,</h3>
            <p>A student has applied for the job vacancy: <strong>{job.Title}</strong> (Company: {job.CompanyName}).</p>
            <hr/>
            <h4>Applicant Details:</h4>
            <ul>
                <li><strong>Full Name:</strong> {student.FullName}</li>
                <li><strong>Email:</strong> {student.Email}</li>
                <li><strong>Faculty:</strong> {student.Faculty}</li>
            </ul>
            <p>You can review the student's CV here: <a href='{finalCvPath}'>View CV File</a></p>";

        try
        {
            await _emailService.SendEmailAsync(emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Email tracking log: " + ex.Message);
        }

        // Send notification to Admin
        await _notificationService.CreateNotificationAsync(
            "New Job Application",
            $"Student {student.FullName} applied for job vacancy: {job.Title}.",
            "JobApplication",
            null,
            null,
            null,
            jobId
        );

        var responseDto = new JobApplicationResponseDto
        {
            Id = application.Id,
            JobOpportunityId = jobId,
            JobTitle = job.Title,
            CompanyName = job.CompanyName,
            StudentId = student.Id,
            StudentFullName = student.FullName,
            StudentEmail = student.Email,
            StudentFaculty = student.Faculty ?? "",
            CvFilePath = finalCvPath,
            AppliedAt = application.AppliedAt
        };

        return new ApiResponse<JobApplicationResponseDto> { Success = true, Message = "Your application submitted successfully.", Data = responseDto };
    }

    // 5. عرض الأدمن لجميع الوظائف في لوحة التحكم
    public async Task<ApiResponse<IEnumerable<JobOpportunityResponseDto>>> GetAllJobsForAdminAsync()
    {
        var jobs = await _context.JobOpportunities.OrderByDescending(j => j.CreatedAt).ToListAsync();
        var result = jobs.Select(j => new JobOpportunityResponseDto
        {
            Id = j.Id,
            Title = j.Title,
            CompanyName = j.CompanyName,
            CompanyEmail = j.CompanyEmail,
            Description = j.Description,
            Requirements = j.Requirements,
            Location = j.Location,
            SalaryRange = j.SalaryRange,
            Type = j.Type.ToString(),
            TargetFaculty = j.TargetFaculty,
            IsApproved = j.IsApproved,
            CreatedAt = j.CreatedAt,
            Deadline = j.Deadline,
            TotalApplicants = _context.JobApplications.Count(a => a.JobOpportunityId == j.Id)
        }).ToList();

        return new ApiResponse<IEnumerable<JobOpportunityResponseDto>> { Success = true, Data = result };
    }

    // 6. عرض الأدمن للمتقدمين لوظيفة معينة
    public async Task<ApiResponse<IEnumerable<JobApplicationResponseDto>>> GetJobApplicationsAsync(int jobId)

    {
        var applications = await _context.JobApplications
            .Include(a => a.JobOpportunity)
            .Include(a => a.Student)
            .Where(a => a.JobOpportunityId == jobId)
            .Select(a => new JobApplicationResponseDto
            {
                Id = a.Id,
                JobOpportunityId = a.JobOpportunityId,
                JobTitle = a.JobOpportunity != null ? a.JobOpportunity.Title : "",
                CompanyName = a.JobOpportunity != null ? a.JobOpportunity.CompanyName : "",
                StudentId = a.StudentId,
                StudentFullName = a.Student != null ? a.Student.FullName : "",
                StudentEmail = a.Student != null ? a.Student.Email : "",
                StudentFaculty = a.Student != null ? (a.Student.Faculty ?? "") : "",
                CvFilePath = a.CvFilePath ?? "",
                AppliedAt = a.AppliedAt
            }).ToListAsync();

        return new ApiResponse<IEnumerable<JobApplicationResponseDto>> { Success = true, Data = applications };
    }

    public async Task<ApiResponse<IEnumerable<JobApplicationResponseDto>>> GetApplicationsByStudentIdAsync(int studentId)
    {
        var applications = await _context.JobApplications
            .Include(a => a.JobOpportunity)
            .Include(a => a.Student)
            .Where(a => a.StudentId == studentId)
            .Select(a => new JobApplicationResponseDto
            {
                Id = a.Id,
                JobOpportunityId = a.JobOpportunityId,
                JobTitle = a.JobOpportunity != null ? a.JobOpportunity.Title : "",
                CompanyName = a.JobOpportunity != null ? a.JobOpportunity.CompanyName : "",
                StudentId = a.StudentId,
                StudentFullName = a.Student != null ? a.Student.FullName : "",
                StudentEmail = a.Student != null ? a.Student.Email : "",
                StudentFaculty = a.Student != null ? (a.Student.Faculty ?? "") : "",
                CvFilePath = a.CvFilePath ?? "",
                AppliedAt = a.AppliedAt
            }).ToListAsync();

        return new ApiResponse<IEnumerable<JobApplicationResponseDto>> { Success = true, Data = applications };
    }

    public async Task<ApiResponse<IEnumerable<JobApplicationResponseDto>>> GetStudentApplicationsAsync(string studentEmail)
{
    var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
    if (student == null)
    {
        return new ApiResponse<IEnumerable<JobApplicationResponseDto>> { Success = false, Message = "Student not found." };
    }
    var applications = await _context.JobApplications
        .Include(a => a.JobOpportunity)
        .Where(a => a.StudentId == student.Id)
        .Select(a => new JobApplicationResponseDto
        {
            Id = a.Id,
            JobOpportunityId = a.JobOpportunityId,
            JobTitle = a.JobOpportunity != null ? a.JobOpportunity.Title : "",
            CompanyName = a.JobOpportunity != null ? a.JobOpportunity.CompanyName : "",
            StudentId = a.StudentId,
            StudentFullName = student.FullName,
            StudentEmail = student.Email,
            StudentFaculty = student.Faculty ?? "",
            CvFilePath = a.CvFilePath ?? "",
            AppliedAt = a.AppliedAt
        })
        .ToListAsync();
    return new ApiResponse<IEnumerable<JobApplicationResponseDto>> { Success = true, Data = applications };
}

public async Task<ApiResponse<string>> CancelApplicationAsync(string email, int applicationId)
{
    var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
    if (student == null) return new ApiResponse<string> { Success = false, Message = "Student not found." };

    var application = await _context.JobApplications
        .FirstOrDefaultAsync(a => a.StudentId == student.Id && a.Id == applicationId);

    if (application == null)
        return new ApiResponse<string> { Success = false, Message = "Application not found." };

    // JobApplications don't currently have a Status field, so we just allow them to cancel.
    _context.JobApplications.Remove(application);
    await _context.SaveChangesAsync();

    return new ApiResponse<string> { Success = true, Message = "تم إزالة الطلب بنجاح." };
}

}