using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UCCD_App.Models;

namespace UCCD_App.Context;

public static class SeedData
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task RoleSeed(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Student", "Registered", "Partial Registered" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task AdminSeed(UserManager<ApplicationUser> userManager)
    {
        var existingAdmin = await userManager.FindByEmailAsync("amiraselem2004@gmail.com");

        if (existingAdmin == null)
        {
            var admin = new ApplicationUser
            {
                FullName = "amira selim",
                Email = "amiraselem2004@gmail.com",
                UserName = "amiraselem2004",
                PhoneNumber = "1234567890",
            };
            var result = await userManager.CreateAsync(admin, "Amira@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }

    public static async Task MockDataSeed(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        string contentRootPath)
    {
        var dataPath = Path.Combine(contentRootPath, "Context", "MockData");

        await SeedUsers(userManager, roleManager, Path.Combine(dataPath, "user.json"));
        await SeedStudents(
            context,
            Path.Combine(dataPath, "user.json"),
            Path.Combine(dataPath, "student.json"));
        await SeedCourses(context, Path.Combine(dataPath, "course.json"));
        await SeedVolunteers(context, Path.Combine(dataPath, "volunteer.json"));

        // ====== 🏢 نداء الميثود الجديدة للوظائف بنفس نظامك بالظبط ======
        await SeedJobs(context, Path.Combine(dataPath, "job.json"));
        await SeedTestimonials(context, Path.Combine(dataPath, "testimonial.json"));
    }

    private static async Task SeedUsers(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        string filePath)
    {
        var users = LoadJson<UserMock>(filePath);

        foreach (var item in users)
        {
            if (string.IsNullOrWhiteSpace(item.Email) || string.IsNullOrWhiteSpace(item.Password))
            {
                continue;
            }

            var existing = await userManager.FindByEmailAsync(item.Email);
            if (existing != null)
            {
                continue;
            }

            var user = new ApplicationUser
            {
                FullName = item.FullName ?? string.Empty,
                Email = item.Email,
                UserName = string.IsNullOrWhiteSpace(item.UserName) ? item.Email : item.UserName,
                PhoneNumber = item.PhoneNumber ?? string.Empty,
                Gender = item.Gender,
                Faculty = item.Faculty,
                NationalID = item.NationalID,
                GraduationYear = item.GraduationYear
            };

            var result = await userManager.CreateAsync(user, item.Password);
            if (result.Succeeded && !string.IsNullOrWhiteSpace(item.Role))
            {
                if (!await roleManager.RoleExistsAsync(item.Role))
                {
                    await roleManager.CreateAsync(new IdentityRole(item.Role));
                }

                await userManager.AddToRoleAsync(user, item.Role);
            }
        }
    }

    private static async Task SeedCourses(AppDbContext context, string filePath)
    {
        var courses = LoadJson<CourseMock>(filePath);
        if (courses.Count == 0)
        {
            return;
        }

        foreach (var item in courses)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                continue;
            }

            var exists = await context.Courses.AnyAsync(c => c.Name == item.Name);
            if (exists)
            {
                continue;
            }

            if (!Enum.TryParse(item.Type, true, out Models.Type courseType))
            {
                courseType = Models.Type.Training;
            }

            context.Courses.Add(new Course
            {
                Name = item.Name,
                StartDate = NormalizeToUtc(item.StartDate) ?? DateTime.UtcNow,
                Duration = item.Duration,
                Capacity = item.Capacity,
                Price = item.Price,
                CertificationFee = item.CertificationFee,
                Type = courseType
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedVolunteers(AppDbContext context, string filePath)
    {
        var volunteers = LoadJson<VolunteerMock>(filePath);
        if (volunteers.Count == 0)
        {
            return;
        }

        foreach (var item in volunteers)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                continue;
            }

            var exists = await context.VolunteerOpportunities.AnyAsync(v => v.Title == item.Title);
            if (exists)
            {
                continue;
            }

            context.VolunteerOpportunities.Add(new VolunteerOpportunity
            {
                Title = item.Title,
                Description = item.Description ?? string.Empty,
                Committee = item.Committee ?? string.Empty,
                RequiredCount = item.RequiredCount,
                Deadline = NormalizeToUtc(item.Deadline) ?? DateTime.UtcNow.AddDays(7),
                IsActive = item.IsActive
            });
        }

        await context.SaveChangesAsync();
    }

    // ====== 🏢 ميثود السيدنج الجديدة للـ Jobs من ملف الـ JSON ======
    private static async Task SeedJobs(AppDbContext context, string filePath)
    {
        var jobs = LoadJson<JobMock>(filePath);
        if (jobs.Count == 0)
        {
            return;
        }

        foreach (var item in jobs)
        {
            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.CompanyName))
            {
                continue;
            }

            var exists = await context.JobOpportunities.AnyAsync(j => j.Title == item.Title && j.CompanyName == item.CompanyName);
            if (exists)
            {
                continue;
            }

            context.JobOpportunities.Add(new JobOpportunity
            {
                Title = item.Title,
                CompanyName = item.CompanyName,
                CompanyEmail = item.CompanyEmail ?? string.Empty,
                Description = item.Description ?? string.Empty,
                Requirements = item.Requirements ?? string.Empty,
                Location = item.Location ?? string.Empty,
                SalaryRange = item.SalaryRange,
                TargetFaculty = item.TargetFaculty ?? string.Empty,
                IsApproved = item.IsApproved,
                CreatedAt = DateTime.UtcNow,
                Deadline = NormalizeToUtc(item.Deadline)
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedTestimonials(AppDbContext context, string filePath)
    {
        var testimonials = LoadJson<TestimonialMock>(filePath);
        if (testimonials.Count == 0)
        {
            return;
        }

        foreach (var item in testimonials)
        {
            if (string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.Content))
            {
                continue;
            }

            var exists = await context.Testimonials.AnyAsync(t => t.Name == item.Name && t.Content == item.Content);
            if (exists)
            {
                continue;
            }

            context.Testimonials.Add(new Testimonial
            {
                Name = item.Name,
                Role = item.Role ?? string.Empty,
                Content = item.Content,
                AvatarUrl = item.AvatarUrl,
                IsApproved = item.IsApproved,
                CreatedAt = NormalizeToUtc(item.CreatedAt) ?? DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedStudents(
        AppDbContext context,
        string usersFilePath,
        string studentsFilePath)
    {
        var users = LoadJson<UserMock>(usersFilePath);
        var students = LoadJson<StudentMock>(studentsFilePath);

        var studentUsers = users
            .Where(u => !string.IsNullOrWhiteSpace(u.Email))
            .Where(u => string.Equals(u.Role, "Student", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (studentUsers.Count == 0)
        {
            return;
        }

        var studentByEmail = students
            .Where(s => !string.IsNullOrWhiteSpace(s.Email))
            .GroupBy(s => s.Email!.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var existingStudents = await context.Students.ToListAsync();
        var existingByEmail = existingStudents
            .Where(s => !string.IsNullOrWhiteSpace(s.Email))
            .GroupBy(s => s.Email.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var user in studentUsers)
        {
            var email = user.Email!.Trim();
            var merged = MergeStudent(user, studentByEmail.TryGetValue(email, out var extra) ? extra : null);

            if (existingByEmail.TryGetValue(email, out var existing))
            {
                existing.FullName = merged.FullName;
                existing.Email = merged.Email;
                existing.Phone = merged.Phone;
                existing.Gender = merged.Gender;
                existing.Faculty = merged.Faculty;
                existing.NationalID = merged.NationalID;
                existing.GraduationYear = merged.GraduationYear;
                continue;
            }

            context.Students.Add(new Student
            {
                Email = merged.Email,
                FullName = merged.FullName,
                Phone = merged.Phone,
                Gender = merged.Gender,
                Faculty = merged.Faculty,
                NationalID = merged.NationalID,
                GraduationYear = merged.GraduationYear
            });
        }

        await context.SaveChangesAsync();
    }

    private static List<T> LoadJson<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
    }

    private static DateTime? NormalizeToUtc(DateTime? value)
    {
        if (value == null)
        {
            return null;
        }

        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }

    // ====== 🏢 كلاس الـ Mock المساعد للوظائف ======
    private sealed class JobMock
    {
        public string? Title { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyEmail { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? Location { get; set; }
        public decimal? SalaryRange { get; set; }
        public string? TargetFaculty { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? Deadline { get; set; }
    }

    private sealed class VolunteerMock
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Committee { get; set; }
        public int RequiredCount { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsActive { get; set; }
    }
    private sealed class TestimonialMock
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Content { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    private sealed class UserMock
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Faculty { get; set; }
        public string? NationalID { get; set; }
        public string? GraduationYear { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
    }

    private sealed class CourseMock
    {
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public decimal CertificationFee { get; set; }
        public string? Type { get; set; }
    }

    private sealed class StudentMock
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Faculty { get; set; }
        public string? NationalID { get; set; }
        public string? GraduationYear { get; set; }
    }

    private static Student MergeStudent(UserMock user, StudentMock? extra)
    {
        return new Student
        {
            Email = (user.Email ?? string.Empty).Trim(),
            FullName = Coalesce(user.FullName, extra?.FullName),
            Phone = Coalesce(user.PhoneNumber, extra?.Phone),
            Gender = Coalesce(user.Gender, extra?.Gender),
            Faculty = Coalesce(user.Faculty, extra?.Faculty),
            NationalID = Coalesce(user.NationalID, extra?.NationalID),
            GraduationYear = Coalesce(user.GraduationYear, extra?.GraduationYear)
        };
    }

    private static string Coalesce(string? primary, string? fallback)
    {
        return !string.IsNullOrWhiteSpace(primary)
            ? primary
            : (fallback ?? string.Empty);
    }
}