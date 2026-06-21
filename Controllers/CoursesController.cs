using Microsoft.AspNetCore.Mvc;
using UCCD_App.Dto;
using UCCD_App.Models;
using UCCD_App.Repo;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Type = UCCD_App.Models.Type;

namespace UCCD_App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IGenericRepo<Course> courseRepo;

    public CoursesController(IGenericRepo<Course> courseRepo)
    {
        this.courseRepo = courseRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetAll()
    {
        var courses = await courseRepo.GetAllAsync();

        return Ok(courses.Select(c => new CourseResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Capacity = c.Capacity,
            CertificationFee = c.CertificationFee,
            Duration = c.Duration,
            Price = c.Price,
            StartDate = c.StartDate,
            Type = c.Type.ToString(),
            Instructor = c.Instructor,
        }));
    }

    // ✅ GET /api/courses/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseResponseDto>> GetByID(int id)
    {
        var course = await courseRepo.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        // رجّعي DTO مش الـ entity
        return Ok(new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            Capacity = course.Capacity,
            CertificationFee = course.CertificationFee,
            Duration = course.Duration,
            Price = course.Price,
            StartDate = course.StartDate,
            Type = course.Type.ToString(),
            Instructor = course.Instructor,
        });
    }

    // ✅ POST /api/courses
    // ✅ يرجّع response فيه Id
    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseResponseDto>> Create(CreateCourseDto courseDto)
    {
        var course = new Course
        {
            Name = courseDto.Name,
            Capacity = courseDto.Capacity,
            CertificationFee = courseDto.CertificationFee,
            Duration = courseDto.Duration,
            Price = courseDto.Price,
            StartDate = courseDto.StartDate.HasValue
                ? NormalizeToUtc(courseDto.StartDate.Value)
                : null,
            Type = (Type)Enum.Parse(typeof(Type), courseDto.Type, true),
            Instructor = courseDto.Instructor
        };

        await courseRepo.AddAsync(course);

        var response = new CourseResponseDto
        {
            Id = course.Id, // ✅ هنا الـ ID بعد الإضافة
            Name = course.Name,
            Capacity = course.Capacity,
            CertificationFee = course.CertificationFee,
            Duration = course.Duration,
            Price = course.Price,
            StartDate = course.StartDate,
            Type = course.Type.ToString(),
            Instructor = course.Instructor,
        };

        // ✅ الأفضل: 201 Created + Location Header
        return CreatedAtAction(nameof(GetByID), new { id = course.Id }, response);
    }

    // ✅ PUT /api/courses/5  (id في endpoint)
    [HttpPut("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseResponseDto>> Update(int id, UpdateCourseDto courseDto)
    {
        var course = await courseRepo.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        course.Name = courseDto.Name;
        course.Capacity = courseDto.Capacity;
        course.CertificationFee = courseDto.CertificationFee;
        course.Duration = courseDto.Duration;
        course.Price = courseDto.Price;
        course.StartDate = NormalizeToUtc(courseDto.StartDate);
        course.Type = (Type)Enum.Parse(typeof(Type), courseDto.Type, true);
        course.Instructor = courseDto.Instructor;

        courseRepo.Update(course);

        // ✅ رجّعي response شامل id
        return Ok(new CourseResponseDto
        {
            Id = course.Id,
            Name = course.Name,
            Capacity = course.Capacity,
            CertificationFee = course.CertificationFee,
            Duration = course.Duration,
            Price = course.Price,
            StartDate = course.StartDate,
            Type = course.Type.ToString(),
            Instructor = course.Instructor,
        });
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        await courseRepo.Delete(id);
        return Ok(true);
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
