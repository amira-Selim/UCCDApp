namespace UCCD_App.Dto;

public class UserTokenResponseDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Token { get; set; } = "";
    public bool RequirePasswordChange { get; set; } = false;
}
