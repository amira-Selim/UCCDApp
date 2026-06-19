using System;

namespace UCCD_App.Dto;

public class JobOpportunityResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string CompanyEmail { get; set; } = "";
    public string Description { get; set; } = "";
    public string Requirements { get; set; } = "";
    public string Location { get; set; } = "";
    public decimal? SalaryRange { get; set; }
    public string TargetFaculty { get; set; } = "";
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public int TotalApplicants { get; set; } // بونص للأدمن عشان يشوف كام طالب قدم
}