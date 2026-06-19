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

    // 3. الأدمن بيشوف الطلاب اللي قدموا على وظيفة معينة بالـ CVs بتاعتهم
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/applications")]
    public async Task<IActionResult> GetJobApplications(int id)
    {
        var response = await _jobBoardService.GetJobApplicationsAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // ==========================================
    // 📢 ألوية الطلاب فقط (Student Endpoints)
    // ==========================================

    // 4. عرض الوظائف المتاحة للطالب (مع تشيك بوكس الفلترة بالكلية)
    [Authorize(Roles = "Student")]
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
    [Authorize(Roles = "Student")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(int id)
    {
        var response = await _jobBoardService.GetJobByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    // 6. تقديم الطالب على وظيفة
    [Authorize(Roles = "Student")]
    [HttpPost("{id}/apply")]
    public async Task<IActionResult> ApplyForJob(int id, [FromBody] ApplyJobDto dto)
    {
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email not found in token." });

        var response = await _jobBoardService.ApplyForJobAsync(studentEmail, id, dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}