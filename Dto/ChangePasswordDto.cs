using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto;

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = "";

    [Required]
    public string NewPassword { get; set; } = "";
}
