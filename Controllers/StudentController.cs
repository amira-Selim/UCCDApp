using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // الأدمن فقط هو اللي يقدر يوصل للكنترولر ده
public class StudentsController : ControllerBase
{
    private readonly IProfileService _profileService;

    public StudentsController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    // الحصول على قائمة بكل الطلاب ببياناتهم الكاملة
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _profileService.GetAllStudentsAsync();
        return Ok(result);
    }

    // الحصول على بيانات طالب محدد باستخدام الـ ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _profileService.GetStudentByIdAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}