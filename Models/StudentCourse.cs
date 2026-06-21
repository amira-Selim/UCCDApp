using System.ComponentModel.DataAnnotations.Schema;

namespace UCCD_App.Models;

public class StudentCourse
{
    public int StudentId { get; set; }
    public int CouresId { get; set; }

    public StudentStatus StudentStatus { get; set; } = StudentStatus.Pending;
    public DateTime? EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }
    [ForeignKey("CouresId")]
    public virtual Course? Course { get; set; }
}
