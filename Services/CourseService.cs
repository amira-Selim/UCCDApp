using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Dto;
using UCCD_App.Models;
using UCCD_App.Repo;
using Type = UCCD_App.Models.Type;

namespace UCCD_App.Services;

public class CourseService : ICourseService
{
    private readonly IGenericRepo<Course> _courseRepo;
    private readonly AppDbContext _context;

    public CourseService(IGenericRepo<Course> courseRepo, AppDbContext context)
    {
        _courseRepo = courseRepo;
        _context = context;
    }

    public async Task<IEnumerable<CourseResponseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepo.GetAllAsync();

        var result = new List<CourseResponseDto>();
        foreach (var c in courses)
        {
            var pendingCount = await _context.StudentCourses
                .CountAsync(sc => sc.CouresId == c.Id && sc.StudentStatus == StudentStatus.Pending);

            result.Add(new CourseResponseDto
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
                PendingCount = pendingCount
            });
        }
        return result;
    }

    public async Task<CourseResponseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _courseRepo.GetByIdAsync(id);
        if (course == null) return null;

        var pendingCount = await _context.StudentCourses
            .CountAsync(sc => sc.CouresId == course.Id && sc.StudentStatus == StudentStatus.Pending);

        return new CourseResponseDto
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
            PendingCount = pendingCount
        };
    }

    public async Task<CourseResponseDto> CreateCourseAsync(CreateCourseDto courseDto)
    {
        var course = new Course
        {
            Name = courseDto.Name,
            Capacity = courseDto.Capacity,
            CertificationFee = courseDto.CertificationFee,
            Duration = courseDto.Duration,
            Price = courseDto.Price,
            StartDate = courseDto.StartDate.HasValue ? NormalizeToUtc(courseDto.StartDate.Value) : null,
            Type = (Type)Enum.Parse(typeof(Type), courseDto.Type, true),
            Instructor = courseDto.Instructor
        };

        await _courseRepo.AddAsync(course);

        return new CourseResponseDto
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
            PendingCount = 0
        };
    }

    public async Task<CourseResponseDto?> UpdateCourseAsync(int id, UpdateCourseDto courseDto)
    {
        var course = await _courseRepo.GetByIdAsync(id);
        if (course == null) return null;

        course.Name = courseDto.Name;
        course.Capacity = courseDto.Capacity;
        course.CertificationFee = courseDto.CertificationFee;
        course.Duration = courseDto.Duration;
        course.Price = courseDto.Price;
        course.StartDate = NormalizeToUtc(courseDto.StartDate);
        course.Type = (Type)Enum.Parse(typeof(Type), courseDto.Type, true);
        course.Instructor = courseDto.Instructor;

        _courseRepo.Update(course);

        var pendingCount = await _context.StudentCourses
            .CountAsync(sc => sc.CouresId == course.Id && sc.StudentStatus == StudentStatus.Pending);

        return new CourseResponseDto
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
            PendingCount = pendingCount
        };
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _courseRepo.GetByIdAsync(id);
        if (course == null) return false;
        
        await _courseRepo.Delete(id);
        return true;
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
