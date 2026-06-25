using System.Collections.Generic;
using System.Threading.Tasks;
using UCCD_App.Dto;

namespace UCCD_App.Services
{
    public interface IAiService
    {
        Task<AiResponseDto> GenerateCoverLetterAsync(string studentEmail, int jobId);
        Task<IEnumerable<JobOpportunityResponseDto>> GetRecommendedJobsAsync(string studentEmail);
        Task<IEnumerable<CourseResponseDto>> GetRecommendedCoursesAsync(string studentEmail, AiCourseRecommendationRequestDto request);
    }
}
