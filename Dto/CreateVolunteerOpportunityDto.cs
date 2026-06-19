using System;

namespace UCCD_App.Dto;

public class CreateVolunteerOpportunityDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Committee { get; set; } = ""; // Logistics | Media | PR | IT
    public int RequiredCount { get; set; }
    public DateTime Deadline { get; set; }
}