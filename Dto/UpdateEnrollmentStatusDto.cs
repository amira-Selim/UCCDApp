using UCCD_App.Models;

public class UpdateEnrollmentStatusDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public StudentStatus Status { get; set; }
}

