namespace UCCD_App.Dto
{
    // Used by GET /api/StudentCourse/student/{studentId}/courses (Admin only)
    // Represents one course a given student is enrolled in, for the Student
    // Details panel's "Registered Courses" section.
    public class StudentRegisteredCourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? Instructor { get; set; }
        public DateTime? StartDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
