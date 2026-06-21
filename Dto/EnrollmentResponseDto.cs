namespace UCCD_App.Dto
{
    public class EnrollmentResponseDto
    {
        public int StudentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool RequiresPayment { get; set; }   // ← السطر الجديد

        public CourseDto? CourseDetails { get; set; } // أضفنا هذا الحقل ليظهر في الـ Add
    }
}