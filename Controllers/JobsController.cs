using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobBoardService _jobBoardService;

    public JobsController(IJobBoardService jobBoardService)
    {
        _jobBoardService = jobBoardService;
    }

    // ==========================================
    // 🛠️ ألوية الأدمن فقط (Admin Endpoints)
    // ==========================================

    // 1. الأدمن بيضيف الوظيفة بنفسه من لوحة التحكم
    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobOpportunityDto dto)
    {
        var response = await _jobBoardService.CreateJobOpportunityByAdminAsync(dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 2. عرض جدول كل الوظائف للأدمن
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-all")]
    public async Task<IActionResult> GetAllJobsForAdmin()
    {
        var response = await _jobBoardService.GetAllJobsForAdminAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 3. الأدمن أو الشركة بيشوفوا الطلاب اللي قدموا على وظيفة معينة بالـ CVs بتاعتهم
    [Authorize(Roles = "Admin,Company")]
    [HttpGet("{id}/applications")]
    public async Task<IActionResult> GetJobApplications(int id)
    {
        // For Company role, verify the job belongs to them
        if (User.IsInRole("Company"))
        {
            var jobResponse = await _jobBoardService.GetJobByIdAsync(id);
            if (!jobResponse.Success || jobResponse.Data == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Job not found." });
            }
            var companyEmail = User.FindFirstValue(ClaimTypes.Email);
            if (jobResponse.Data.CompanyEmail != companyEmail)
            {
                return Forbid(); // Return 403 Forbidden
            }
        }

        var response = await _jobBoardService.GetJobApplicationsAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // ==========================================
    // 🏢 ألوية الشركات فقط (Company Endpoints)
    // ==========================================

    [Authorize(Roles = "Company")]
    [HttpPost("company-create")]
    public async Task<IActionResult> CreateJobByCompany([FromBody] CreateJobOpportunityDto dto)
    {
        var companyEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(companyEmail)) return Unauthorized();

        var response = await _jobBoardService.CreateJobOpportunityByCompanyAsync(companyEmail, dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [Authorize(Roles = "Company")]
    [HttpGet("my-company-jobs")]
    public async Task<IActionResult> GetMyCompanyJobs()
    {
        var companyEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(companyEmail)) return Unauthorized();

        var response = await _jobBoardService.GetCompanyJobsAsync(companyEmail);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // ==========================================
    // 📢 ألوية الطلاب فقط (Student Endpoints)
    // ==========================================

    // 4. عرض الوظائف المتاحة للطالب (مع تشيك بوكس الفلترة بالكلية)
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableJobs([FromQuery] bool filterByMyFacultyOnly = false)
    {
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email not found in token." });

        var response = await _jobBoardService.GetApprovedJobsForStudentsAsync(studentEmail, filterByMyFacultyOnly);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 5. عرض تفاصيل وظيفة محددة للطالب بالـ ID
    [Authorize(Roles = "Student,Admin,Company")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(int id)
    {
        var response = await _jobBoardService.GetJobByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    // 6. تقديم الطالب على وظيفة
    [Authorize(Roles = "Student")]
    [HttpPost("{id}/apply")]
    public async Task<IActionResult> ApplyForJob(int id, [FromForm] ApplyJobDto dto)
    {
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email not found in token." });

        var response = await _jobBoardService.ApplyForJobAsync(studentEmail, id, dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // GET: api/Jobs/my-applications
    [Authorize(Roles = "Student")]
    [HttpGet("my-applications")]
    public async Task<IActionResult> GetMyApplications()
    {
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email not found in token." });
        var response = await _jobBoardService.GetStudentApplicationsAsync(studentEmail);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [Authorize(Roles = "Student")]
    [HttpDelete("applications/{applicationId}/cancel")]
    public async Task<IActionResult> CancelApplication(int applicationId)
    {
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email not found in token." });

        var response = await _jobBoardService.CancelApplicationAsync(studentEmail, applicationId);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}