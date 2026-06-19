using System;

namespace UCCD_App.Dto;

public class VolunteerApplicationResponseDto
{
    public int Id { get; set; }
    public int OpportunityId { get; set; }
    public string OpportunityTitle { get; set; } = "";
    public int StudentId { get; set; }
    public string StudentFullName { get; set; } = "";
    public string StudentEmail { get; set; } = "";
    public string Motivation { get; set; } = "";
    public string Skills { get; set; } = "";
    public string Status { get; set; } = ""; // Pending | Approved | Rejected
    public DateTime AppliedAt { get; set; }
}