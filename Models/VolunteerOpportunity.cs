using System;
using System.Collections.Generic;

namespace UCCD_App.Models;

public class VolunteerOpportunity : BaseEntity
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Committee { get; set; } = ""; // Logistics | Media | PR | IT
    public int RequiredCount { get; set; }        // العدد المطلوب من المتطوعين
    public DateTime? Deadline { get; set; }       // آخر ميعاد للتقديم
    public bool IsActive { get; set; } = true;    // هل الفرصة متاحة حالياً؟

    // علاقة One-to-Many: الفرصة الواحدة ليها كذا طلب تقديم
    public virtual ICollection<VolunteerApplication> Applications { get; set; } 
        = new HashSet<VolunteerApplication>();
}