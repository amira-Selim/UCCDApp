using System;
using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        public string? UserId { get; set; } // Null if global/admin
        public string? RecipientEmail { get; set; }
        public string? RecipientRole { get; set; }
        public int? RelatedCourseId { get; set; }
        public int? RelatedVolunteerId { get; set; }
        public int? RelatedJobId { get; set; }

        public virtual Course? RelatedCourse { get; set; }
        public virtual VolunteerOpportunity? RelatedVolunteer { get; set; }
        public virtual JobOpportunity? RelatedJob { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error
    }
}