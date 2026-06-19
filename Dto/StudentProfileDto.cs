namespace UCCD_App.Dto
{
    public class StudentProfileDto
    {
         public int Id { get; set; } // ضيفي السطر ده هنا

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string GraduationYear { get; set; } = string.Empty;
        public string NationalID { get; set; } = string.Empty;
        // ممكن نزود عدد الكورسات المسجل فيها كـ Dashboard معلومة سريعة
        // البيانات اللي الطالب يقدر يضيفها أو يعدلها وقت ما يحب
        public string? Education { get; set; }
        public string? Skills { get; set; }
        public string? Interests { get; set; }
        public string? CareerGoal { get; set; }
        public int EnrolledCoursesCount { get; set; }
    }
}