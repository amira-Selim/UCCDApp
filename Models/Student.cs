namespace UCCD_App.Models;

public class Student:BaseEntity
{
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";

    //complete profile fields
    public string Gender { get; set; } = "";
    public string Faculty { get; set; } = "";
    public string NationalID { get; set; } = "";
    public string GraduationYear { get; set; } = "";

    //
        public string? Education { get; set; }
        public string? Skills { get; set; }
        public string? Interests { get; set; }
        public string? CareerGoal { get; set; }

    public ICollection<StudentCourse> StudentCourses { get; set; }
    =new HashSet<StudentCourse>();
    public virtual ICollection<VolunteerApplication> VolunteerApplications { get; set; } 
    = new HashSet<VolunteerApplication>();

}
