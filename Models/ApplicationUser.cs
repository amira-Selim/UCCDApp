using Microsoft.AspNetCore.Identity;

namespace UCCD_App.Models;

public class ApplicationUser:IdentityUser
{
    public string FullName { get; set; } = "";

    // 
    public string? Gender { get; set; } = "";
    public string? Faculty { get; set; } = "";
    public string? NationalID { get; set; } = "";
    public string? GraduationYear { get; set; } = "";
    
    public bool RequirePasswordChange { get; set; } = false;
}
