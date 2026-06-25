using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto
{
    public class AiCoverLetterRequestDto
    {
        [Required]
        public int JobId { get; set; }
    }

    public class AiResponseDto
    {
        public string ResultText { get; set; }
    }

    public class AiCourseRecommendationRequestDto
    {
        public string FieldOfInterest { get; set; }
        public string CareerGoal { get; set; }
        public string CurrentLevel { get; set; }
    }
}
