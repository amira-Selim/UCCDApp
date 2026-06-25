using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
