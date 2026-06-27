using System.Collections.Generic;

namespace UCCD_App.Dto;

public class ApplicantProfileDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Faculty { get; set; } = "";
    public string GraduationYear { get; set; } = "";
    public string Education { get; set; } = "";
    public string Skills { get; set; } = "";
    public string Interests { get; set; } = "";
    public string CareerGoal { get; set; } = "";

    public List<ApplicantCourseDto> Courses { get; set; } = new List<ApplicantCourseDto>();
    public List<ApplicantVolunteerDto> VolunteerWork { get; set; } = new List<ApplicantVolunteerDto>();
}

public class ApplicantCourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public string Status { get; set; } = "";
}

public class ApplicantVolunteerDto
{
    public int VolunteerOpportunityId { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
}
