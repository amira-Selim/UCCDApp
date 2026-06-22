using System.Collections.Generic;
using System.Threading.Tasks;
using UCCD_App.Dto;

namespace UCCD_App.Services;

public interface IJobBoardService
{
    // حاجات الأدمن (إضافة وعرض المتقدمين)
    Task<ApiResponse<JobOpportunityResponseDto>> CreateJobOpportunityByAdminAsync(CreateJobOpportunityDto dto);
    Task<ApiResponse<IEnumerable<JobOpportunityResponseDto>>> GetAllJobsForAdminAsync();
    Task<ApiResponse<IEnumerable<JobApplicationResponseDto>>> GetJobApplicationsAsync(int jobId);

    // حاجات الطلاب (استعراض والتقديم)
    Task<ApiResponse<IEnumerable<JobOpportunityResponseDto>>> GetApprovedJobsForStudentsAsync(string studentEmail, bool filterByMyFacultyOnly);
    Task<ApiResponse<JobOpportunityResponseDto>> GetJobByIdAsync(int id);
    Task<ApiResponse<JobApplicationResponseDto>> ApplyForJobAsync(string studentEmail, int jobId, ApplyJobDto dto);

    Task<ApiResponse<IEnumerable<JobApplicationResponseDto>>> GetStudentApplicationsAsync(string studentEmail);
}