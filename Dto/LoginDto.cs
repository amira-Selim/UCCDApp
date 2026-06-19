using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto;

public class LoginDto
{
    [EmailAddress, Required]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
