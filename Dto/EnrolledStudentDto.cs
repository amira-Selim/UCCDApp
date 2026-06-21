namespace UCCD_App.Dto
{
    // Used by GET /api/StudentCourse/course/{courseId}/students (Admin only)
    // Represents one student enrolled in a given course, for the Course
    // Details page's "Enrolled Students" table.
    public class EnrolledStudentDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
