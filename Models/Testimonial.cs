using System;
using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Role { get; set; } 
        [Required]
        public string Content { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
