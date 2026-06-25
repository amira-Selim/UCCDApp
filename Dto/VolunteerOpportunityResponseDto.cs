using System;

namespace UCCD_App.Dto;

public class VolunteerOpportunityResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Committee { get; set; } = "";
    public int RequiredCount { get; set; }
    public int CurrentApprovedCount { get; set; } // الفيتشر اللي قولنا عليها: عشان الفرونت يعرض كام واحد اتقبل لحد دلوقتي
    public int PendingApplicantsCount { get; set; } // الفيتشر الجديدة: كام واحد معلق
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; }
}