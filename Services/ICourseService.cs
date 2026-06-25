using System.Collections.Generic;
using System.Threading.Tasks;
using UCCD_App.Dto;

namespace UCCD_App.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync();
    Task<CourseResponseDto?> GetCourseByIdAsync(int id);
    Task<CourseResponseDto> CreateCourseAsync(CreateCourseDto courseDto);
    Task<CourseResponseDto?> UpdateCourseAsync(int id, UpdateCourseDto courseDto);
    Task<bool> DeleteCourseAsync(int id);
}
