using System;
using System.Collections.Generic;

namespace UCCD_App.Models;

public class JobOpportunity : BaseEntity
{
    public string Title { get; set; } = "";          // عنوان الوظيفة (مثلا: Junior .NET Developer)
    public string CompanyName { get; set; } = "";    // اسم الشركة المتعاقدة
    public string CompanyEmail { get; set; } = "";   // إيميل الشركة (اللي هيتبعت عليه الـ CV)
    public string Description { get; set; } = "";    // الوصف الوظيفي
    public string Requirements { get; set; } = "";   // الشروط والمهارات المطلوبة
    public string Location { get; set; } = "";       // مكان العمل (Cairo, Remote...)
    public decimal? SalaryRange { get; set; }        // الراتب (اختياري)
    public JobType Type { get; set; }                // نوع الوظيفة
    public string TargetFaculty { get; set; } = "";  // الكلية المستهدفة للفلترة (مثل: Computers and Information)
    
    public JobStatus Status { get; set; } = JobStatus.Pending; // حالة الوظيفة
    public string? RejectionReason { get; set; }             // سبب الرفض إن وُجد
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? Deadline { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; } 
        = new HashSet<JobApplication>();
}