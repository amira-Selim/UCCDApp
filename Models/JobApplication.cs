using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCCD_App.Models;

public class JobApplication : BaseEntity
{
    public int JobOpportunityId { get; set; }
    public int StudentId { get; set; }
    
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public string? CvFilePath { get; set; } // ملف الـ CV المرفوع خصيصاً للوظيفة دي (اختياري)
    public string? CoverLetter { get; set; }

    [ForeignKey("JobOpportunityId")]
    public virtual JobOpportunity? JobOpportunity { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }
}