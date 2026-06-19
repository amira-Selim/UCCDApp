using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto
{
    public class CreateMessageDto
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string IssueType { get; set; } = "";

        [Required]
        public string Content { get; set; } = "";
    }

    public class MessageResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string IssueType { get; set; } = "";

        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public bool IsArchived { get; set; }
    }
}