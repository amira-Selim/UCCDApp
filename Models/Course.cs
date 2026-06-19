namespace UCCD_App.Models;

public class Course:BaseEntity
{
    public string Name { get; set; } = "";
    public DateTime? StartDate { get; set; }
    public int Duration { get; set; }
    public int Capacity { get; set; }
    public decimal Price { get; set; }
    public decimal CertificationFee { get; set; }
    public Type Type { get; set; }
    //
    public virtual ICollection<StudentCourse> StudentCourses { get; set; }
    = new HashSet<StudentCourse>();
}
