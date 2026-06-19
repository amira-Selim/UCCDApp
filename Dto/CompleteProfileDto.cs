using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto
{
    public class CompleteProfileDto
    {
        [Required(ErrorMessage = "الجنس مطلوب")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "الكلية مطلوبة")]
        public string Faculty { get; set; } = string.Empty;

        [Required(ErrorMessage = "سنة التخرج مطلوبة")]
        public string GraduationYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "الرقم القومي يجب أن يكون 14 رقم")]
        public string NationalID { get; set; } = string.Empty;
    }
}