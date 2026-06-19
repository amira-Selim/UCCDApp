using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; } = "";
    [Required]
    public string LastName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, Phone]
    public string PhoneNumber { get; set; } = "";

    [Required, RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
    ErrorMessage = "Password must be 8-15 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; set; } = "";
}