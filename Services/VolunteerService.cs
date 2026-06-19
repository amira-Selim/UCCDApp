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

public class VolunteerService : IVolunteerService
{
    private readonly IGenericRepo<VolunteerOpportunity> _opportunityRepo;
    private readonly IGenericRepo<VolunteerApplication> _applicationRepo;
    private readonly AppDbContext _context;

    public VolunteerService(
        IGenericRepo<VolunteerOpportunity> opportunityRepo,
        IGenericRepo<VolunteerApplication> applicationRepo,
        AppDbContext context)
    {
        _opportunityRepo = opportunityRepo;
        _applicationRepo = applicationRepo;
        _context = context;
    }

    // 1. إنشاء فرصة تطوعية جديدة (ترجع كاملة ببياناتها)
    public async Task<ApiResponse<VolunteerOpportunityResponseDto>> CreateOpportunityAsync(CreateVolunteerOpportunityDto dto)
    {
        var opportunity = new VolunteerOpportunity
        {
            Title = dto.Title,
            Description = dto.Description,
            Committee = dto.Committee,
            RequiredCount = dto.RequiredCount,
            Deadline = dto.Deadline.ToUniversalTime(),
            IsActive = true
        };

        await _opportunityRepo.AddAsync(opportunity);

        var responseDto = new VolunteerOpportunityResponseDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            Committee = opportunity.Committee,
            RequiredCount = opportunity.RequiredCount,
            Deadline = opportunity.Deadline,
            IsActive = opportunity.IsActive,
            CurrentApprovedCount = 0
        };

        return new ApiResponse<VolunteerOpportunityResponseDto> 
        { 
            Success = true, 
            Message = "Opportunity created successfully", 
            Data = responseDto 
        };
    }

    // 2. تعديل فرصة تطوعية (ترجع كاملة بعد التعديل)
    public async Task<ApiResponse<VolunteerOpportunityResponseDto>> UpdateOpportunityAsync(int id, CreateVolunteerOpportunityDto dto)
    {
        var opportunity = await _opportunityRepo.GetByIdAsync(id);
        if (opportunity == null)
            return new ApiResponse<VolunteerOpportunityResponseDto> { Success = false, Message = "Opportunity not found" };

        opportunity.Title = dto.Title;
        opportunity.Description = dto.Description;
        opportunity.Committee = dto.Committee;
        opportunity.RequiredCount = dto.RequiredCount;
        opportunity.Deadline = dto.Deadline.ToUniversalTime();

        _opportunityRepo.Update(opportunity);

        var approvedCount = await _context.VolunteerApplications.CountAsync(a => a.OpportunityId == id && a.Status == VolunteerStatus.Approved);

        var responseDto = new VolunteerOpportunityResponseDto
        {
            Id = opportunity.Id,
            Title = opportunity.Title,
            Description = opportunity.Description,
            Committee = opportunity.Committee,
            RequiredCount = opportunity.RequiredCount,
            Deadline = opportunity.Deadline,
            IsActive = opportunity.IsActive,
            CurrentApprovedCount = approvedCount
        };

        return new ApiResponse<VolunteerOpportunityResponseDto> 
        { 
            Success = true, 
            Message = "Opportunity updated successfully", 
            Data = responseDto 
        };
    }

    // 3. عرض كل الفرص مع الفلترة
    public async Task<ApiResponse<IEnumerable<VolunteerOpportunityResponseDto>>> GetAllOpportunitiesAsync(bool? isActive)
    {
        var opportunities = await _opportunityRepo.GetAllAsync();
        
        if (isActive.HasValue)
        {
            opportunities = opportunities.Where(o => o.IsActive == isActive.Value).ToList();
        }

        var result = new List<VolunteerOpportunityResponseDto>();
        foreach (var o in opportunities)
        {
            var approvedCount = await _context.VolunteerApplications.CountAsync(a => a.OpportunityId == o.Id && a.Status == VolunteerStatus.Approved);
            result.Add(new VolunteerOpportunityResponseDto
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                Committee = o.Committee,
                RequiredCount = o.RequiredCount,
                Deadline = o.Deadline,
                IsActive = o.IsActive,
                CurrentApprovedCount = approvedCount
            });
        }

        return new ApiResponse<IEnumerable<VolunteerOpportunityResponseDto>> 
        { 
            Success = true, 
            Data = result.AsEnumerable() 
        };
    }

    // 4. عرض فرصة محددة بالـ ID
    public async Task<ApiResponse<VolunteerOpportunityResponseDto>> GetOpportunityByIdAsync(int id)
    {
        var o = await _opportunityRepo.GetByIdAsync(id);
        if (o == null)
            return new ApiResponse<VolunteerOpportunityResponseDto> { Success = false, Message = "Opportunity not found" };

        var approvedCount = await _context.VolunteerApplications.CountAsync(a => a.OpportunityId == o.Id && a.Status == VolunteerStatus.Approved);

        var dto = new VolunteerOpportunityResponseDto
        {
            Id = o.Id,
            Title = o.Title,
            Description = o.Description,
            Committee = o.Committee,
            RequiredCount = o.RequiredCount,
            Deadline = o.Deadline,
            IsActive = o.IsActive,
            CurrentApprovedCount = approvedCount
        };

        return new ApiResponse<VolunteerOpportunityResponseDto> { Success = true, Data = dto };
    }

    // 5. تقديم الطالب على فرصة (بدون ApplicationUser وبـ FullName)
    public async Task<ApiResponse<VolunteerApplicationResponseDto>> ApplyForOpportunityAsync(string studentEmail, int opportunityId, ApplyVolunteerDto dto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
        if (student == null)
            return new ApiResponse<VolunteerApplicationResponseDto> { Success = false, Message = "Student profile not found. Please complete your profile first." };

        var opportunity = await _opportunityRepo.GetByIdAsync(opportunityId);
        if (opportunity == null)
            return new ApiResponse<VolunteerApplicationResponseDto> { Success = false, Message = "Opportunity not found" };

        var alreadyApplied = await _context.VolunteerApplications.AnyAsync(a => a.OpportunityId == opportunityId && a.StudentId == student.Id);
        if (alreadyApplied)
            return new ApiResponse<VolunteerApplicationResponseDto> { Success = false, Message = "You have already applied for this opportunity." };

        if (opportunity.Deadline.HasValue && opportunity.Deadline.Value < DateTime.UtcNow)
            return new ApiResponse<VolunteerApplicationResponseDto> { Success = false, Message = "The deadline for this opportunity has passed." };

        var application = new VolunteerApplication
        {
            OpportunityId = opportunityId,
            StudentId = student.Id,
            Motivation = dto.Motivation,
            Skills = dto.Skills,
            Status = VolunteerStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        await _applicationRepo.AddAsync(application);

        var responseDto = new VolunteerApplicationResponseDto
        {
            Id = application.Id,
            OpportunityId = opportunityId,
            OpportunityTitle = opportunity.Title,
            StudentId = student.Id,
            StudentFullName = student.FullName, 
            StudentEmail = studentEmail,
            Motivation = application.Motivation,
            Skills = application.Skills,
            Status = application.Status.ToString(),
            AppliedAt = application.AppliedAt
        };

        return new ApiResponse<VolunteerApplicationResponseDto> 
        { 
            Success = true, 
            Message = "Application submitted successfully", 
            Data = responseDto 
        };
    }

    // 6. عرض طلبات الطالب الحالي (معدلة وآمنة 100% من الـ Warning)
    public async Task<ApiResponse<IEnumerable<VolunteerApplicationResponseDto>>> GetStudentApplicationsAsync(string studentEmail)
    {
        var applications = await _context.VolunteerApplications
            .Include(a => a.Opportunity)
            .Include(a => a.Student)
            .Where(a => a.Student != null && a.Student.Email == studentEmail)
            .Select(a => new VolunteerApplicationResponseDto
            {
                Id = a.Id,
                OpportunityId = a.OpportunityId,
                OpportunityTitle = a.Opportunity != null ? a.Opportunity.Title : "",
                StudentId = a.StudentId,
                StudentFullName = a.Student != null ? a.Student.FullName : "",
                StudentEmail = studentEmail,
                Motivation = a.Motivation,
                Skills = a.Skills,
                Status = a.Status.ToString(),
                AppliedAt = a.AppliedAt
            }).ToListAsync();

        return new ApiResponse<IEnumerable<VolunteerApplicationResponseDto>> 
        { 
            Success = true, 
            Data = applications.AsEnumerable() 
        };
    }

    // 7. عرض كل المتقدمين لفرصة معينة (للأدمن)
    public async Task<ApiResponse<IEnumerable<VolunteerApplicationResponseDto>>> GetApplicationsByOpportunityIdAsync(int opportunityId)
    {
        var applications = await _context.VolunteerApplications
            .Include(a => a.Opportunity)
            .Include(a => a.Student)
            .Where(a => a.OpportunityId == opportunityId)
            .Select(a => new VolunteerApplicationResponseDto
            {
                Id = a.Id,
                OpportunityId = a.OpportunityId,
                OpportunityTitle = a.Opportunity != null ? a.Opportunity.Title : "",
                StudentId = a.StudentId,
                StudentFullName = a.Student != null ? a.Student.FullName : "",
                StudentEmail = a.Student != null ? a.Student.Email : "",
                Motivation = a.Motivation,
                Skills = a.Skills,
                Status = a.Status.ToString(),
                AppliedAt = a.AppliedAt
            }).ToListAsync();

        return new ApiResponse<IEnumerable<VolunteerApplicationResponseDto>> 
        { 
            Success = true, 
            Data = applications.AsEnumerable() 
        };
    }

    // 8. قبول أو رفض طلب تطوع (مع حماية الـ Capacity ويرجع كامل ببياناته)
    public async Task<ApiResponse<VolunteerApplicationResponseDto>> UpdateApplicationStatusAsync(UpdateVolunteerStatusDto dto)
    {
        var application = await _context.VolunteerApplications
            .Include(a => a.Opportunity)
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.Id == dto.ApplicationId);

        if (application == null)
            return new ApiResponse<VolunteerApplicationResponseDto> { Success = false, Message = "Application not found" };

        var newStatus = (VolunteerStatus)Enum.Parse(typeof(VolunteerStatus), dto.NewStatus, true);

        if (newStatus == VolunteerStatus.Approved && application.Status != VolunteerStatus.Approved)
        {
            var approvedCount = await _context.VolunteerApplications.CountAsync(a => a.OpportunityId == application.OpportunityId && a.Status == VolunteerStatus.Approved);
            if (application.Opportunity != null && approvedCount >= application.Opportunity.RequiredCount)
            {
                return new ApiResponse<VolunteerApplicationResponseDto> { Success = false, Message = "Cannot approve. This opportunity has already reached its required capacity." };
            }
        }

        application.Status = newStatus;
        _applicationRepo.Update(application);

        var responseDto = new VolunteerApplicationResponseDto
        {
            Id = application.Id,
            OpportunityId = application.OpportunityId,
            OpportunityTitle = application.Opportunity != null ? application.Opportunity.Title : "",
            StudentId = application.StudentId,
            StudentFullName = application.Student != null ? application.Student.FullName : "",
            StudentEmail = application.Student != null ? application.Student.Email : "",
            Motivation = application.Motivation,
            Skills = application.Skills,
            Status = application.Status.ToString(),
            AppliedAt = application.AppliedAt
        };

        return new ApiResponse<VolunteerApplicationResponseDto> 
        { 
            Success = true, 
            Message = $"Application status updated to {dto.NewStatus} successfully", 
            Data = responseDto 
        };
    }
}