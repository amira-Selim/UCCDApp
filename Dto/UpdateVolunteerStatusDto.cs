namespace UCCD_App.Dto;

public class UpdateVolunteerStatusDto
{
    public int ApplicationId { get; set; }
    public string NewStatus { get; set; } = ""; // Approved or Rejected
}