using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCCD_App.Models;

public class VolunteerApplication : BaseEntity
{
    public int OpportunityId { get; set; }
    public int StudentId { get; set; } // مربوط بجدول الـ Student الحالي عندك

    public string Motivation { get; set; } = ""; // سبب الرغبة في التطوع
    public string Skills { get; set; } = "";       // المهارات المتعلقة بالفرصة

    public VolunteerStatus Status { get; set; } = VolunteerStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("OpportunityId")]
    public virtual VolunteerOpportunity? Opportunity { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }
}