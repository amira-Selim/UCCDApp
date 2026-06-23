namespace UCCD_App.Dto
{
    public class StudentEnrollmentDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public CourseDto? CourseDetails { get; set; }
    }

    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        // يمكنك إضافة أي حقول إضافية هنا (مثل ImageUrl, Duration, etc.)
    }
}