using System;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UCCD_App.Models;

namespace UCCD_App.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {

    }

    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<StudentCourse> StudentCourses { get; set; }
    public DbSet<JobOpportunity> JobOpportunities { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    // جداول المتطوعين اللي ضفتيها
    public DbSet<VolunteerOpportunity> VolunteerOpportunities { get; set; }
    public DbSet<VolunteerApplication> VolunteerApplications { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // course
        modelBuilder.Entity<Course>()
            .Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Course>()
            .Property(c => c.Type)
            .HasConversion(
                status => status.ToString(),
                status => (UCCD_App.Models.Type)Enum.Parse(typeof(UCCD_App.Models.Type), status));

        modelBuilder.Entity<Course>()
            .Property(c => c.CertificationFee)
            .HasColumnType("decimal(18,2)");

        // studentCourse - enum conversion
        modelBuilder.Entity<StudentCourse>()
            .Property(sc => sc.StudentStatus)
            .HasConversion(
                st => st.ToString(),
                st => (StudentStatus)Enum.Parse(typeof(StudentStatus), st));

        // composite key
        modelBuilder.Entity<StudentCourse>()
            .HasKey("StudentId", "CouresId");

        // relations
        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Student)
            .WithMany(s => s.StudentCourses)
            .HasForeignKey(sc => sc.StudentId);

        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Course)
            .WithMany(c => c.StudentCourses)
            .HasForeignKey(sc => sc.CouresId);

        // ==========================================
        // الـ Fluent API الجديد الخاص بالمتطوعين (الخطوة التانية اللي تهمنا)
        // ==========================================

        // تحويل الـ Enum لـ string عشان يتسيف في الداتا بيز 
        modelBuilder.Entity<VolunteerApplication>()
            .Property(a => a.Status)
            .HasConversion(
                st => st.ToString(),
                st => (VolunteerStatus)Enum.Parse(typeof(VolunteerStatus), st));

        // تفعيل الـ Cascade Delete (لو الفرصة اتمسحت، طلباتها تتمسح تلقائياً)
        modelBuilder.Entity<VolunteerApplication>()
            .HasOne(a => a.Opportunity)
            .WithMany(o => o.Applications)
            .HasForeignKey(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notifications - Cascade Delete Handlers
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.RelatedCourse)
            .WithMany()
            .HasForeignKey(n => n.RelatedCourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.RelatedVolunteer)
            .WithMany()
            .HasForeignKey(n => n.RelatedVolunteerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.RelatedJob)
            .WithMany()
            .HasForeignKey(n => n.RelatedJobId)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}