using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string IssueType { get; set; } = "";

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public bool IsArchived { get; set; } = false;
    }
}