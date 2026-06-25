using Microsoft.AspNetCore.Mvc;
using UCCD_App.Dto;
using UCCD_App.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UCCD_App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetAll()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(courses);
    }

    // ✅ GET /api/courses/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseResponseDto>> GetByID(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound();

        return Ok(course);
    }

    // ✅ POST /api/courses
    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseResponseDto>> Create(CreateCourseDto courseDto)
    {
        var response = await _courseService.CreateCourseAsync(courseDto);
        return CreatedAtAction(nameof(GetByID), new { id = response.Id }, response);
    }

    // ✅ PUT /api/courses/5
    [HttpPut("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseResponseDto>> Update(int id, UpdateCourseDto courseDto)
    {
        var response = await _courseService.UpdateCourseAsync(id, courseDto);
        if (response == null)
            return NotFound();

        return Ok(response);
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var result = await _courseService.DeleteCourseAsync(id);
        if (!result) return NotFound();
        return Ok(true);
    }
}
