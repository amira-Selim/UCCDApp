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
public class VolunteersController : ControllerBase
{
    private readonly IVolunteerService _volunteerService;

    public VolunteersController(IVolunteerService volunteerService)
    {
        _volunteerService = volunteerService;
    }

    // ==========================================
    // 📢 ألوية الطلاب (Student Endpoints)
    // ==========================================

    // 1. عرض كل الفرص التطوعية (متاح للطلاب وللجميع لمعرفة المتاح)
    [HttpGet("opportunities")]
    public async Task<IActionResult> GetOpportunities([FromQuery] bool? isActive)
    {
        var response = await _volunteerService.GetAllOpportunitiesAsync(isActive);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 2. عرض تفاصيل فرصة معينة بالـ ID
    [HttpGet("opportunities/{id}")]
    public async Task<IActionResult> GetOpportunityById(int id)
    {
        var response = await _volunteerService.GetOpportunityByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    // 3. تقديم الطالب على فرصة تطوعية معينة
    [Authorize(Roles = "Student")] 
    [HttpPost("opportunities/{id}/apply")]
    public async Task<IActionResult> ApplyForOpportunity(int id, [FromBody] ApplyVolunteerDto dto)
    {
        // سحب إيميل الطالب تلقائياً من الـ Token الحالي اللي عامل بيه Login
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email claim not found in token." });

        var response = await _volunteerService.ApplyForOpportunityAsync(studentEmail, id, dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 4. عرض طلبات التطوع الخاصة بالطالب الحالي (صفحة "طلباتي")
    [Authorize(Roles = "Student")]
    [HttpGet("my-applications")]
    public async Task<IActionResult> GetMyApplications()
    {
        var studentEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(studentEmail))
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User email claim not found in token." });

        var response = await _volunteerService.GetStudentApplicationsAsync(studentEmail);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // ==========================================
    // 🛠️ ألوية الأدمن (Admin Endpoints)
    // ==========================================

    // 5. إنشاء فرصة تطوعية جديدة (ترجع كاملة ببياناتها)
    [Authorize(Roles = "Admin")]
    [HttpPost("opportunities")]
    public async Task<IActionResult> CreateOpportunity([FromBody] CreateVolunteerOpportunityDto dto)
    {
        var response = await _volunteerService.CreateOpportunityAsync(dto);
        if (response.Success && response.Data != null)
        {
            return CreatedAtAction(nameof(GetOpportunityById), new { id = response.Data.Id }, response);
        }
        return BadRequest(response);
    }

    // 6. تعديل فرصة تطوعية (ترجع كاملة بعد التعديل)
    [Authorize(Roles = "Admin")]
    [HttpPut("opportunities/{id}")]
    public async Task<IActionResult> UpdateOpportunity(int id, [FromBody] CreateVolunteerOpportunityDto dto)
    {
        var response = await _volunteerService.UpdateOpportunityAsync(id, dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 7. عرض كل الطلاب اللي قدموا على فرصة معينة
    [Authorize(Roles = "Admin")]
    [HttpGet("opportunities/{id}/applications")]
    public async Task<IActionResult> GetOpportunityApplications(int id)
    {
        var response = await _volunteerService.GetApplicationsByOpportunityIdAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 8. قبول أو رفض طلب تطوع (تغيير الحالة مع حماية الـ Capacity وترجع كاملة)
    [Authorize(Roles = "Admin")]
    [HttpPut("applications/update-status")]
    public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateVolunteerStatusDto dto)
    {
        var response = await _volunteerService.UpdateApplicationStatusAsync(dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}