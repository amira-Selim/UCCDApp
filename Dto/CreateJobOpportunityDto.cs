using System;
using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto;

public class CreateJobOpportunityDto
{
    [Required]
    public string Title { get; set; } = "";

    [Required]
    public string CompanyName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string CompanyEmail { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [Required]
    public string Requirements { get; set; } = "";

    [Required]
    public string Location { get; set; } = "";

    public decimal? SalaryRange { get; set; }

    [Required]
    public string Type { get; set; } = "";

    [Required]
    public string TargetFaculty { get; set; } = ""; // الكلية المستهدفة

    public DateTime? Deadline { get; set; }
}